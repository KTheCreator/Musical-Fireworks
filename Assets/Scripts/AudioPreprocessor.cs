/*The Code below belongs to the Jesse Scam (https://github.com/jesse-scam/algorithmic-beat-mapping-unity/blob/master/Assets/Lib/Internal/SpectralFluxAnalyzer.cs)
 and is used purely for preprocessing the audio tracks. To see the article explaining the */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPreprocessorInfo
{
    public float time, spectralFlux, threshold, prunedSpectralFlux;
    public bool isBeat;
}

public class AudioPreprocessor
{
    int numSamples = 1024;
    float thresholdMultiplier = 1.5f;
    int thresholdWindowSize = 50;
    public List<AudioPreprocessorInfo> spectralFluxSamples;
    float[] currSpec;
    float[] prevSpec;
    int indexToProcess;

    public AudioPreprocessor()
    {
        spectralFluxSamples = new List<AudioPreprocessorInfo>();
        indexToProcess = thresholdWindowSize / 2;
        currSpec = new float[numSamples];
        prevSpec = new float[numSamples];
    }
    public void setCurrSpectrum(float[] spectrum)
    {
        currSpec.CopyTo(prevSpec, 0);
        spectrum.CopyTo(currSpec, 0);
    }
    float calculateRectifiedSpectralFlux()
    {
        float sum = 0f;
        for(int i = 0; i < numSamples; i++)
        {
            sum += Mathf.Max(0f, currSpec[i] - prevSpec[i]);
        }
        return sum;
    }
    float getFluxThreshold(int spectralFluxIndex)
    {
        int windowStartIndex = Mathf.Max(0, spectralFluxIndex - thresholdWindowSize / 2);
        int windowEndIndex = Mathf.Min(spectralFluxSamples.Count - 1, spectralFluxIndex + thresholdWindowSize / 2);
        float sum = 0f;
        for(int i = windowStartIndex;i < windowEndIndex; i++)
        {
            sum += spectralFluxSamples[i].spectralFlux;
        }
        float avg = sum / (windowEndIndex - windowStartIndex);
        return avg * thresholdMultiplier;
    }
    float getPrunedSpectralFlux(int spectralFluxIndex)
    {
        return Mathf.Max(0f, spectralFluxSamples[spectralFluxIndex].spectralFlux - spectralFluxSamples[spectralFluxIndex].threshold);
    }
    bool isPeak(int spectralFluxIndex)
    {
        if (spectralFluxSamples[spectralFluxIndex].prunedSpectralFlux > spectralFluxSamples[spectralFluxIndex + 1].prunedSpectralFlux &&
            spectralFluxSamples[spectralFluxIndex].prunedSpectralFlux > spectralFluxSamples[spectralFluxIndex - 1].prunedSpectralFlux) {
            return true;
        }
        else { return false; }
    }
    public void analyzeSpectrum(float[] spectrum, float time)
    {
        setCurrSpectrum(spectrum);
        AudioPreprocessorInfo currInfo = new AudioPreprocessorInfo();
        currInfo.time = time;
        currInfo.spectralFlux = calculateRectifiedSpectralFlux();
        spectralFluxSamples.Add(currInfo);
        if (spectralFluxSamples.Count >= thresholdWindowSize)
        {
            spectralFluxSamples[indexToProcess].threshold = getFluxThreshold(indexToProcess);
            spectralFluxSamples[indexToProcess].prunedSpectralFlux = getPrunedSpectralFlux(indexToProcess);
            int indextoDetectPeak = indexToProcess - 1;
            bool currPeak = isPeak(indextoDetectPeak);
            if (currPeak) { spectralFluxSamples[indextoDetectPeak].isBeat = true; }
            indexToProcess++;
        }
        else { Debug.Log(string.Format("Not ready yet.  At spectral flux sample size of {0} growing to {1}", spectralFluxSamples.Count, thresholdWindowSize)); }

    }

    public void logSample(int indexToLog)
    {
        int windowStart = Mathf.Max(0, indexToLog - thresholdWindowSize / 2);
        int windowEnd = Mathf.Min(spectralFluxSamples.Count - 1, indexToLog + thresholdWindowSize / 2);
        Debug.Log(string.Format(
            "Peak detected at song time {0} with pruned flux of {1} ({2} over thresh of {3}).\n" +
            "Thresh calculated on time window of {4}-{5} ({6} seconds) containing {7} samples.",
            spectralFluxSamples[indexToLog].time,
            spectralFluxSamples[indexToLog].prunedSpectralFlux,
            spectralFluxSamples[indexToLog].spectralFlux,
            spectralFluxSamples[indexToLog].threshold,
            spectralFluxSamples[windowStart].time,
            spectralFluxSamples[windowEnd].time,
            spectralFluxSamples[windowEnd].time - spectralFluxSamples[windowStart].time,
            windowEnd - windowStart
        ));
    }

}
