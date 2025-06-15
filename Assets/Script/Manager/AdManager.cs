using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    [Header("AdMob Settings")]
    [SerializeField] private string androidAppId = "ca-app-pub-3940256099942544~3347511713"; // Test App ID
    [SerializeField] private string androidBannerId = "ca-app-pub-3940256099942544/6300978111"; // Test Banner ID
    [SerializeField] private string androidInterstitialId = "ca-app-pub-3940256099942544/1033173712"; // Test Interstitial ID

    [Header("Debug Settings")]
    [SerializeField] private bool enableTestAds = true;
    [SerializeField] private List<string> testDeviceIds = new List<string>();

    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private bool isInterstitialLoaded = false;
    private AdSize adSize;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAds();

            bannerView = new BannerView(androidBannerId, adSize, AdPosition.Bottom);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void InitializeAds()
    {
        // Test cihazlarý ayarla
        if (enableTestAds)
        {
            List<string> deviceIds = new List<string>();

            // Kendi test cihaz ID'nizi buraya ekleyin
            if (testDeviceIds.Count > 0)
            {
                deviceIds.AddRange(testDeviceIds);
            }

            // Test configuration
            RequestConfiguration requestConfiguration = new RequestConfiguration
            {
                TestDeviceIds = deviceIds
            };
            MobileAds.SetRequestConfiguration(requestConfiguration);
        }

        // AdMob'u baþlat
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("AdMob initialized successfully");
            LoadBannerAd();
            LoadInterstitialAd();
        });
    }

    #region Banner Ads
    private void LoadBannerAd()
    {
        // Banner varsa destroy et
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        // Banner boyutu ve pozisyonu
        AdSize adSize = AdSize.Banner; // 320x50
        bannerView = new BannerView(androidBannerId, adSize, AdPosition.Bottom);

        // Banner event'leri
        bannerView.OnBannerAdLoaded += OnBannerAdLoaded;
        bannerView.OnBannerAdLoadFailed += OnBannerAdLoadFailed;

        // Ad request oluþtur
        AdRequest request = new AdRequest();

        // Banner'ý yükle
        bannerView.LoadAd(request);
    }

    public void ShowBannerAd()
    {
        if (bannerView != null)
        {
            bannerView.Show();
        }
    }

    public void HideBannerAd()
    {
        if (bannerView != null)
        {
            bannerView.Hide();
        }
    }

    private void OnBannerAdLoaded()
    {
        Debug.Log("Banner ad loaded successfully");
    }

    private void OnBannerAdLoadFailed(LoadAdError error)
    {
        Debug.LogError("Banner ad failed to load: " + error.GetMessage());
    }
    #endregion

    #region Interstitial Ads
    private void LoadInterstitialAd()
    {
        // Mevcut interstitial'ý temizle
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        // Ad request oluþtur
        AdRequest request = new AdRequest();

        // Interstitial'ý yükle
        InterstitialAd.Load(androidInterstitialId, request, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Interstitial ad failed to load: " + error?.GetMessage());
                isInterstitialLoaded = false;
                return;
            }

            Debug.Log("Interstitial ad loaded successfully");
            interstitialAd = ad;
            isInterstitialLoaded = true;

            // Event'leri kaydet
            RegisterInterstitialEvents(interstitialAd);
        });
    }

    private void RegisterInterstitialEvents(InterstitialAd interstitialAd)
    {
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad opened");
            // Oyunu duraklat
            Time.timeScale = 0f;
            AudioListener.pause = true;
        };

        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad closed");
            // Oyunu devam ettir
            Time.timeScale = 1f;
            AudioListener.pause = false;

            // Yeni interstitial yükle
            LoadInterstitialAd();
        };

        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to show: " + error.GetMessage());
            // Oyunu devam ettir
            Time.timeScale = 1f;
            AudioListener.pause = false;

            // Yeni interstitial yükle
            LoadInterstitialAd();
        };
    }

    public void ShowInterstitialAd(Action onAdClosed = null)
    {
        if (interstitialAd != null && isInterstitialLoaded)
        {
            // Kapanma callback'i varsa kaydet
            if (onAdClosed != null)
            {
                interstitialAd.OnAdFullScreenContentClosed += () =>
                {
                    onAdClosed?.Invoke();
                };
            }

            interstitialAd.Show();
            isInterstitialLoaded = false;
        }
        else
        {
            Debug.Log("Interstitial ad not ready");
            // Reklam hazýr deðilse callback'i hemen çaðýr
            onAdClosed?.Invoke();
        }
    }

    public bool IsInterstitialReady()
    {
        return interstitialAd != null && isInterstitialLoaded;
    }
    #endregion

    #region Test Device Management
    public void AddTestDevice(string deviceId)
    {
        if (!testDeviceIds.Contains(deviceId))
        {
            testDeviceIds.Add(deviceId);
            UpdateTestConfiguration();
        }
    }

    public void RemoveTestDevice(string deviceId)
    {
        if (testDeviceIds.Contains(deviceId))
        {
            testDeviceIds.Remove(deviceId);
            UpdateTestConfiguration();
        }
    }

    private void UpdateTestConfiguration()
    {
        if (enableTestAds)
        {
            RequestConfiguration requestConfiguration = new RequestConfiguration
            {
                TestDeviceIds = testDeviceIds
            };
            MobileAds.SetRequestConfiguration(requestConfiguration);
        }
    }

    // Test cihaz ID'nizi loglardan öðrenmek için
    private void Start()
    {
        // Test modunda çalýþýyorken console'da cihaz ID'nizi göreceksiniz
        if (enableTestAds && Debug.isDebugBuild)
        {
            Debug.Log("Test ads enabled. Check console for your device ID.");
        }
    }
    #endregion

    private void OnDestroy()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            HideBannerAd();
        }
        else
        {
            ShowBannerAd();
        }
    }
}