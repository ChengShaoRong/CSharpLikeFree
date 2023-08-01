using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CSharpLike//RongRong : Change namespace to "CSharpLike" or add "using CSharpLike;" in the front.
{
    /// <summary>
    /// RongRong : This class include mothed 'Start/Update', we use 'Update' in place of Coroutine,
    /// we using 'HotUpdateBehaviourUpdate' to bind prefabe.
    /// We add 'Integers' name "m_NumRoundsToWin" in prefab.
    /// We add 'Floats' name "m_StartDelay"/"m_EndDelay" in prefab.
    /// We add 'Game Objects' name "m_CameraControl"/"m_MessageText"/"m_Tanks0"/"m_Tanks1" in prefab.
    /// We add 'Colors' name "m_Tanks0"/"m_Tanks1" in prefab.
    /// </summary>
    public class GameManager : LikeBehaviour //RongRong : Change 'MonoBehaviour' to 'LikeBehaviour'
    {
        public int m_NumRoundsToWin = 5;            // The number of rounds a single player has to win to win the game.
        public float m_StartDelay = 3f;             // The delay between the start of RoundStarting and RoundPlaying phases.
        public float m_EndDelay = 3f;               // The delay between the end of RoundPlaying and RoundEnding phases.
        public CameraControl m_CameraControl;       // Reference to the CameraControl script for control during different phases.
        public Text m_MessageText;                  // Reference to the overlay Text to display winning text, etc.
        public GameObject m_TankPrefab;             // Reference to the prefab the players will control.
        public TankManager[] m_Tanks;               // A collection of managers for enabling and disabling different aspects of the tanks.

        
        private int m_RoundNumber;                  // Which round the game is currently on.
        // RongRong : Not support coroutine in FREE version
        //private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts.
        //private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends.
        private TankManager m_RoundWinner;          // Reference to the winner of the current round.  Used to make an announcement of who won.
        private TankManager m_GameWinner;           // Reference to the winner of the game.  Used to make an announcement of who won.


        const float k_MaxDepenetrationVelocity = float.PositiveInfinity;

        // RongRong : Bind value MUST in Awake before use it due to execute order : Awake -> OnEnable -> Start.
        void Awake()
        {
            // RongRong : Bind value from prefab.
            m_NumRoundsToWin = GetInt("m_NumRoundsToWin", 5);
            m_StartDelay = GetFloat("m_StartDelay", 3f);
            m_EndDelay = GetFloat("m_EndDelay", 3f);
            m_MessageText = GetComponent<Text>("m_MessageText");
            m_TankPrefab = GetGameObject("m_TankPrefab");
            m_Tanks = new TankManager[2];
            for (int i = 0; i < 2; i++)
            {
                TankManager tankManager = new TankManager();
                tankManager.m_SpawnPoint = GetGameObject("m_Tanks"+i).transform;
                tankManager.m_PlayerColor = GetColor("m_Tanks"+i);
                m_Tanks[i] = tankManager;
            }
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                ResourceManager.LoadAudioClipAsync("https://www.csharplike.com/CSharpLikeFreeDemo/AssetBundles/Tank/WebGL/BackgroundMusic.wav", AudioType.WAV, (AudioClip audioClip) =>
                {
                    if (audioClip != null)
                    {
                        gameObject.GetComponent<AudioSource>().clip = audioClip;
                        gameObject.GetComponent<AudioSource>().Play();
                    }
                });
            }
        }

        private void Start()
        {
            m_CameraControl = HotUpdateBehaviour.GetComponentByType(GetGameObject("m_CameraControl"), typeof(CameraControl)) as CameraControl;
            // This line fixes a change to the physics engine.
            Physics.defaultMaxDepenetrationVelocity = k_MaxDepenetrationVelocity;

            // Create the delays so they only have to be made once.
            // RongRong : Not support coroutine in FREE version
            //m_StartWait = new WaitForSeconds (m_StartDelay);
            //m_EndWait = new WaitForSeconds (m_EndDelay);

            SpawnAllTanks();
            SetCameraTargets();

            // Once the tanks have been created and the camera is using them as targets, start the game.
            // RongRong : Change 'StartCoroutine(GameLoop ())' to 'StartCoroutine("GameLoop");'
            mState = 1;
        }

        /// <summary>
        /// RongRong : We use 7 states for in place of coroutine
        /// 0-None start;
        /// 1-RoundStarting;
        /// 2-RoundStartingWait;
        /// 3-RoundPlayingEnter;
        /// 4-RoundPlaying;
        /// 5-RoundEnding;
        /// 6-RoundEndingWait;
        /// </summary>
        int mState = 0;
        /// <summary>
        /// RongRong : We use delta time for in place of 'm_StartWait' and 'm_EndWait'
        /// </summary>
        float mDeltaTime = 0f;
        /// <summary>
        /// RongRong : We use Update for in place of coroutine
        /// </summary>
        /// <param name="deltaTime">Delta time from last update.</param>
        void Update(float deltaTime)
        {
            if (mState == 1)//RoundStarting : do something for starting
            {
                RoundStarting();
                mDeltaTime = 0f;
                mState = 2;
            }
            else if (mState == 2)//RoundPlayingWait: countdown for next state
            {
                mDeltaTime += deltaTime;
                if (mDeltaTime >= m_StartDelay)//Timeout, and then next state
                {
                    mDeltaTime = 0f;
                    mState = 3;
                }
            }
            else if (mState == 3)//RoundPlayingEnter: just enter the RoundPlaying, we initialize here before the RoundPlaying loop.
            {
                // As soon as the round begins playing let the players control the tanks.
                EnableTankControl();

                // Clear the text from the screen.
                m_MessageText.text = string.Empty;

                mState = 4;
            }
            else if (mState == 4)//RoundPlaying: the main game loop here, check for whether this round end.
            {
                if (OneTankLeft())//Once just have one tank left, change to next state
                    mState = 5;
            }
            else if (mState == 5)//RoundEnding : calc the round ending
            {
                RoundEnding();
                mState = 6;
            }
            else if (mState == 6)//RoundEndingWait: countdown for next state
            {
                mDeltaTime += deltaTime;
                if (mDeltaTime >= m_EndDelay)//Timeout, and then next state
                {
                    // This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found.
                    if (m_GameWinner != null)
                    {
                        // If there is a game winner, restart the level.
                        SceneManager.LoadScene("_Complete-Game_HotUpdate");
                    }
                    else
                    {
                        // If there isn't a winner yet, restart this coroutine so the loop continues.
                        mDeltaTime = 0f;
                        mState = 1;
                    }
                }
            }
        }

        private void SpawnAllTanks()
        {
            // For all the tanks...
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                // ... create them, set their player number and references needed for control.
                // RongRong : Change 'Instantiate' to 'GameObject.Instantiate'
                m_Tanks[i].m_Instance =
                    GameObject.Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
                m_Tanks[i].m_PlayerNumber = i + 1;
                m_Tanks[i].Setup();
            }
        }


        private void SetCameraTargets()
        {
            // Create a collection of transforms the same size as the number of tanks.
            Transform[] targets = new Transform[m_Tanks.Length];

            // For each of these transforms...
            for (int i = 0; i < targets.Length; i++)
            {
                // ... set it to the appropriate tank transform.
                targets[i] = m_Tanks[i].m_Instance.transform;
            }

            // These are the targets the camera should follow.
            m_CameraControl.m_Targets = targets;
        }


        private void RoundStarting ()
        {
            // As soon as the round starts reset the tanks and make sure they can't move.
            ResetAllTanks ();
            DisableTankControl ();

            // Snap the camera's zoom and position to something appropriate for the reset tanks.
            m_CameraControl.SetStartPositionAndSize();

            // Increment the round number and display text showing the players what round it is.
            m_RoundNumber++;
            m_MessageText.text = "ROUND " + m_RoundNumber;
        }

        private void RoundEnding ()
        {
            // Stop tanks from moving.
            DisableTankControl ();

            // Clear the winner from the previous round.
            m_RoundWinner = null;

            // See if there is a winner now the round is over.
            m_RoundWinner = GetRoundWinner ();

            // If there is a winner, increment their score.
            if (m_RoundWinner != null)
                m_RoundWinner.m_Wins++;

            // Now the winner's score has been incremented, see if someone has one the game.
            m_GameWinner = GetGameWinner ();

            // Get a message based on the scores and whether or not there is a game winner and display it.
            string message = EndMessage ();
            m_MessageText.text = message;
        }


        // This is used to check if there is one or fewer tanks remaining and thus the round should end.
        private bool OneTankLeft()
        {
            // Start the count of tanks left at zero.
            int numTanksLeft = 0;

            // Go through all the tanks...
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                // ... and if they are active, increment the counter.
                if (m_Tanks[i].m_Instance.activeSelf)
                    numTanksLeft++;
            }

            // If there are one or fewer tanks remaining return true, otherwise return false.
            return numTanksLeft <= 1;
        }
        
        
        // This function is to find out if there is a winner of the round.
        // This function is called with the assumption that 1 or fewer tanks are currently active.
        private TankManager GetRoundWinner()
        {
            // Go through all the tanks...
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                // ... and if one of them is active, it is the winner so return it.
                if (m_Tanks[i].m_Instance.activeSelf)
                    return m_Tanks[i];
            }

            // If none of the tanks are active it is a draw so return null.
            return null;
        }


        // This function is to find out if there is a winner of the game.
        private TankManager GetGameWinner()
        {
            // Go through all the tanks...
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                // ... and if one of them has enough rounds to win the game, return it.
                if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                    return m_Tanks[i];
            }

            // If no tanks have enough rounds to win, return null.
            return null;
        }


        // Returns a string message to display at the end of each round.
        private string EndMessage()
        {
            // By default when a round ends there are no winners so the default end message is a draw.
            string message = "DRAW!";

            // If there is a winner then change the message to reflect that.
            if (m_RoundWinner != null)
                message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

            // Add some line breaks after the initial message.
            message += "\n\n\n\n";

            // Go through all the tanks and add each of their scores to the message.
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
            }

            // If there is a game winner, change the entire message to reflect that.
            if (m_GameWinner != null)
                message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

            return message;
        }


        // This function is used to turn all the tanks back on and reset their positions and properties.
        private void ResetAllTanks()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].Reset();
            }
        }


        private void EnableTankControl()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].EnableControl();
            }
        }


        private void DisableTankControl()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].DisableControl();
            }
        }
        /// <summary>
        /// Back to the main scene
        /// </summary>
        void Back2MainScene()
        {
            Time.timeScale = 1f;//Time scale reset to 1
            SceneManager.LoadScene(0);
        }
    }
}