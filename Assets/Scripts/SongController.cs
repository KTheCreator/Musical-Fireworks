using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Numerics;
using DSPLib;

using Array = System.Array;
public class SongController : MonoBehaviour
{
    int numChannels, numTotalSamples, sampleRate;
    float clipLength;
    float[] multiChannelSamples;
    AudioPreprocessor audPP;
    AudioSource aSource;
    PlotController pController;
    // Start is called before the first frame update
    void Start()
    {
        aSource = GetComponent<AudioSource>();
        audPP = new AudioPreprocessor();
        pController = GameObject.Find("PlotMachine").GetComponent<PlotController>();
        multiChannelSamples = new float[aSource.clip.samples * aSource.clip.channels];
        numChannels = aSource.clip.channels;
        numTotalSamples = aSource.clip.samples;
        clipLength = aSource.clip.length;
        this.sampleRate = aSource.clip.frequency;
        aSource.clip.GetData(multiChannelSamples, 0);
		Debug.Log("GetData: Complete");
        Thread bgThread = new Thread(this.getFullSpectrumThreaded);
		bgThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        int indexToPlot = getIndexFromTime(aSource.time) / 1024;
        pController.updatePlot(audPP.spectralFluxSamples, indexToPlot);

    }
    public int getIndexFromTime(float curTime)
    {
        float lengthPerSample = this.clipLength / (float)this.numTotalSamples;

        return Mathf.FloorToInt(curTime / lengthPerSample);
    }

    public float getTimeFromIndex(int index)
    {
        return ((1f / (float)this.sampleRate) * index);
    }

	public void getFullSpectrumThreaded()
	{
		try
		{
			// We only need to retain the samples for combined channels over the time domain
			float[] preProcessedSamples = new float[this.numTotalSamples];

			int numProcessed = 0;
			float combinedChannelAverage = 0f;
			for (int i = 0; i < multiChannelSamples.Length; i++)
			{
				combinedChannelAverage += multiChannelSamples[i];

				// Each time we have processed all channels samples for a point in time, we will store the average of the channels combined
				if ((i + 1) % this.numChannels == 0)
				{
					preProcessedSamples[numProcessed] = combinedChannelAverage / this.numChannels;
					numProcessed++;
					combinedChannelAverage = 0f;
				}
			}

			Debug.Log("Combine Channels done");
			Debug.Log(preProcessedSamples.Length);

			// Once we have our audio sample data prepared, we can execute an FFT to return the spectrum data over the time domain
			int spectrumSampleSize = 1024;
			int iterations = preProcessedSamples.Length / spectrumSampleSize;

			FFT fft = new FFT();
			fft.Initialize((System.UInt32)spectrumSampleSize);

			Debug.Log(string.Format("Processing {0} time domain samples for FFT", iterations));
			double[] sampleChunk = new double[spectrumSampleSize];
			for (int i = 0; i < iterations; i++)
			{
				// Grab the current 1024 chunk of audio sample data
				Array.Copy(preProcessedSamples, i * spectrumSampleSize, sampleChunk, 0, spectrumSampleSize);

				// Apply our chosen FFT Window
				double[] windowCoefs = DSP.Window.Coefficients(DSP.Window.Type.Hanning, (uint)spectrumSampleSize);
				double[] scaledSpectrumChunk = DSP.Math.Multiply(sampleChunk, windowCoefs);
				double scaleFactor = DSP.Window.ScaleFactor.Signal(windowCoefs);

				// Perform the FFT and convert output (complex numbers) to Magnitude
				Complex[] fftSpectrum = fft.Execute(scaledSpectrumChunk);
				double[] scaledFFTSpectrum = DSPLib.DSP.ConvertComplex.ToMagnitude(fftSpectrum);
				scaledFFTSpectrum = DSP.Math.Multiply(scaledFFTSpectrum, scaleFactor);

				// These 1024 magnitude values correspond (roughly) to a single point in the audio timeline
				float curSongTime = getTimeFromIndex(i) * spectrumSampleSize;

				// Send our magnitude data off to our Spectral Flux Analyzer to be analyzed for peaks
				audPP.analyzeSpectrum(Array.ConvertAll(scaledFFTSpectrum, x => (float)x), curSongTime);
			}

			Debug.Log("Spectrum Analysis done");
			Debug.Log("Background Thread Completed");

		}
		catch (System.Exception e)
		{
			// Catch exceptions here since the background thread won't always surface the exception to the main thread
			Debug.Log(e.ToString());
		}
	}
}
