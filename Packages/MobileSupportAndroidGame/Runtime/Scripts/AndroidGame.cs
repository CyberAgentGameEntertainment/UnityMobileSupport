// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using UnityEngine;

namespace MobileSupport
{
    /// <summary>
    ///     <seealso href="https://developer.android.com/reference/android/app/GameManager">developer.android.com</seealso>
    /// </summary>
    public enum AndroidGameMode
    {
        /// <summary>
        ///     Game mode is not supported for this application.
        ///     <remarks>Added in API level 31</remarks>
        /// </summary>
        Unsupported = 0,

        /// <summary>
        ///     Standard game mode means the platform will use the game's default performance characteristics.
        ///     <remarks>Added in API level 31</remarks>
        /// </summary>
        Standard = 1,

        /// <summary>
        ///     Performance game mode maximizes the game's performance.
        ///     <remarks>Added in API level 31</remarks>
        /// </summary>
        Performance = 2,

        /// <summary>
        ///     Battery game mode will save battery and give longer game play time.
        ///     <remarks>Added in API level 31</remarks>
        /// </summary>
        Battery = 3
    }

    /// <summary>
    ///     <seealso href="https://developer.android.com/reference/android/app/GameState">developer.android.com</seealso>
    /// </summary>
    public enum AndroidGameState
    {
        /// <summary>
        ///     Default Game state is unknown.
        ///     <remarks>Added in API level 33</remarks>
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     No state means that the game is not in active play, for example the user is using the game menu.
        ///     <remarks>Added in API level 33</remarks>
        /// </summary>
        None = 1,

        /// <summary>
        ///     Indicates if the game is in active, but interruptible, game play.
        ///     <remarks>Added in API level 33</remarks>
        /// </summary>
        GamePlayInterruptible = 2,

        /// <summary>
        ///     Indicates if the game is in active user play mode, which is real time and cannot be interrupted.
        ///     <remarks>Added in API level 33</remarks>
        /// </summary>
        GamePlayUninterruptible = 3,

        /// <summary>
        ///     Indicates that the current content shown is not game play related. For example it can be an ad, a web page, a text,
        ///     or a video.
        ///     <remarks>Added in API level 33</remarks>
        /// </summary>
        Content = 4
    }

    public static class AndroidGame
    {
        private static AndroidJavaObject _gameManager;
        private static int _sdkVersion;

        /// <summary>
        ///     <para>Get the user selected game mode for the application.</para>
        ///     <remarks>Added in API level 31</remarks>
        /// </summary>
        /// <returns>User selected <see cref="AndroidGameMode" /></returns>
        public static AndroidGameMode GameMode
        {
            get
            {
                var gameManager = GetGameManager();
                if (gameManager == null) return AndroidGameMode.Unsupported;

                return (AndroidGameMode)gameManager.Call<int>("getGameMode");
            }
        }

        private static AndroidJavaObject GetGameManager()
        {
            if (_gameManager != null) return _gameManager;

            try
            {
                // GameManager is available from API level 31
                if (GetAndroidSdkVersion() < 31) return null;

                using var unityActivity = GetUnityActivity();
                if (unityActivity == null) return null;

                using var gameManagerCls = new AndroidJavaClass("android.app.GameManager");
                _gameManager = unityActivity.Call<AndroidJavaObject>("getSystemService", gameManagerCls);
                return _gameManager;
            }
            catch (AndroidJavaException e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        private static AndroidJavaObject GetUnityActivity()
        {
            using var unityActivityCls = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            return unityActivityCls.GetStatic<AndroidJavaObject>("currentActivity");
        }

        private static int GetAndroidSdkVersion()
        {
#if UNITY_EDITOR
            if (Application.isEditor) return 0;
#endif
#if UNITY_ANDROID
            if (_sdkVersion != 0) return _sdkVersion;

            using var versionCls = new AndroidJavaClass("android.os.Build$VERSION");
            _sdkVersion = versionCls.GetStatic<int>("SDK_INT");
            return _sdkVersion;
#else
            return 0;
#endif
        }

        /// <summary>
        ///     <para>Create a GameState with the specified loading status.</para>
        ///     <remarks>Added in API level 33</remarks>
        /// </summary>
        /// <param name="isLoading">Whether the game is in the loading state.</param>
        /// <param name="gameState">The game state mode.</param>
        public static void SetGameState(bool isLoading, AndroidGameState gameState)
        {
            if (GetAndroidSdkVersion() < 33) return;

            var gameManager = GetGameManager();
            if (gameManager == null) return;

            using var gameStateObj = new AndroidJavaObject("android.app.GameState", isLoading, (int)gameState);
            gameManager.Call("setGameState", gameStateObj);
        }

        /// <summary>
        ///     <para>Create a GameState with the given state variables.</para>
        ///     <remarks>Added in API level 33</remarks>
        /// </summary>
        /// <param name="isLoading">Whether the game is in the loading state.</param>
        /// <param name="gameState">The game state mode.</param>
        /// <param name="label">An optional developer-supplied enum e.g. for the current level.</param>
        /// <param name="quality">An optional developer-supplied enum, e.g. for the current quality level.</param>
        public static void SetGameState(bool isLoading, AndroidGameState gameState, int label, int quality)
        {
            if (GetAndroidSdkVersion() < 33) return;

            var gameManager = GetGameManager();
            if (gameManager == null) return;

            using var gameStateObj =
                new AndroidJavaObject("android.app.GameState", isLoading, (int)gameState, label, quality);
            gameManager.Call("setGameState", gameStateObj);
        }
    }
}
