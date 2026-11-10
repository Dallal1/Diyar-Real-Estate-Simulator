using UnityEngine;

namespace Diyar.Game
{
    public sealed class MainMenuUI : MonoBehaviour
    {
        private PlayerMovement playerMovement;
        private PlayerInteraction playerInteraction;
        private ThirdPersonCamera thirdPersonCamera;

        private GUIStyle panelStyle;
        private GUIStyle titleStyle;
        private GUIStyle subtitleStyle;
        private GUIStyle buttonStyle;
        private GUIStyle secondaryButtonStyle;
        private GUIStyle controlsStyle;
        private GUIStyle footerStyle;

        private Texture2D overlayTexture;
        private Texture2D panelTexture;
        private Texture2D primaryButtonTexture;
        private Texture2D primaryHoverTexture;
        private Texture2D secondaryButtonTexture;
        private Texture2D secondaryHoverTexture;
        private Texture2D accentTexture;

        private bool showControls;
        private bool hasStarted;

        private void Awake()
        {
            playerMovement = FindObjectOfType<PlayerMovement>();
            playerInteraction = FindObjectOfType<PlayerInteraction>();
            thirdPersonCamera = FindObjectOfType<ThirdPersonCamera>();

            SetGameplayEnabled(false);
            Time.timeScale = 0f;
            Cursor.visible = true;
        }

        private void OnGUI()
        {
            EnsureStyles();

            float scale = Mathf.Clamp(Screen.height / 900f, 0.8f, 1f);

            if (hasStarted)
            {
                DrawMenuButton(scale);
                return;
            }

            float panelWidth = Mathf.Min(500f * scale, Screen.width - 32f);
            float panelHeight = Mathf.Min(
                (showControls ? 700f : 560f) * scale,
                Screen.height - 32f);
            Rect panelRect = new Rect(
                (Screen.width - panelWidth) * 0.5f,
                (Screen.height - panelHeight) * 0.5f,
                panelWidth,
                panelHeight);

            Color previousColor = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, 0.76f);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), overlayTexture);
            GUI.color = previousColor;

            GUI.Box(panelRect, GUIContent.none, panelStyle);
            float reservedBottom = showControls ? 80f * scale : 0f;
            GUILayout.BeginArea(new Rect(
                panelRect.x + 28f * scale,
                panelRect.y + 24f * scale,
                panelRect.width - 56f * scale,
                panelRect.height - 48f * scale - reservedBottom));

            GUILayout.Space(12f * scale);
            GUILayout.Label("DIYAR", titleStyle);
            GUILayout.Label("EIN NEUER ANFANG IN DAMASKUS", subtitleStyle);
            GUILayout.Space(14f * scale);
            GUILayout.Box(GUIContent.none, GUIStyle.none, GUILayout.Height(2f * scale));
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), accentTexture);
            GUILayout.Space(20f * scale);

            if (showControls)
            {
                DrawControls(scale);
            }
            else
            {
                DrawMainButtons(scale);
            }

            if (!showControls)
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("DAMASKUS  ·  ALEPPO", footerStyle);
                GUILayout.Space(8f * scale);
            }

            GUILayout.EndArea();

            if (showControls)
            {
                Rect backButtonRect = new Rect(
                    panelRect.x + 28f * scale,
                    panelRect.y + panelRect.height - 68f * scale,
                    panelRect.width - 56f * scale,
                    46f * scale);

                if (GUI.Button(backButtonRect, "ZURÜCK", secondaryButtonStyle))
                {
                    showControls = false;
                }
            }
        }

        private void DrawMainButtons(float scale)
        {
            GUILayout.Space(8f * scale);

            if (GUILayout.Button("SPIELEN", buttonStyle, GUILayout.Height(58f * scale)))
            {
                StartGame();
            }

            GUILayout.Space(12f * scale);

            if (GUILayout.Button("STEUERUNG", secondaryButtonStyle, GUILayout.Height(54f * scale)))
            {
                showControls = true;
            }

            GUILayout.Space(12f * scale);

            if (GUILayout.Button("BEENDEN", secondaryButtonStyle, GUILayout.Height(54f * scale)))
            {
                QuitGame();
            }
        }

        private void DrawControls(float scale)
        {
            GUILayout.Label("STEUERUNG", subtitleStyle);
            GUILayout.Space(12f * scale);
            GUILayout.Label("W A S D      Bewegen", controlsStyle);
            GUILayout.Space(8f * scale);
            GUILayout.Label("Shift          Rennen", controlsStyle);
            GUILayout.Space(8f * scale);
            GUILayout.Label("Leertaste   Springen", controlsStyle);
            GUILayout.Space(8f * scale);
            GUILayout.Label("E                 Interagieren", controlsStyle);
            GUILayout.Space(8f * scale);
            GUILayout.Label("Rechte Maustaste + Maus\nKamera drehen", controlsStyle);
        }

        private void DrawMenuButton(float scale)
        {
            Rect menuButtonRect = new Rect(
                Screen.width - 150f * scale,
                16f * scale,
                134f * scale,
                46f * scale);

            if (GUI.Button(menuButtonRect, "MENÜ", secondaryButtonStyle))
            {
                PauseGame();
            }
        }

        private void PauseGame()
        {
            hasStarted = false;
            showControls = false;
            Time.timeScale = 0f;
            Cursor.visible = true;
            SetGameplayEnabled(false);
        }

        private void StartGame()
        {
            hasStarted = true;
            Time.timeScale = 1f;
            Cursor.visible = true;
            SetGameplayEnabled(true);
        }

        private void SetGameplayEnabled(bool isEnabled)
        {
            if (playerMovement != null)
            {
                playerMovement.enabled = isEnabled;
            }

            if (playerInteraction != null)
            {
                playerInteraction.enabled = isEnabled;
            }

            if (thirdPersonCamera != null)
            {
                thirdPersonCamera.enabled = isEnabled;
            }
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void EnsureStyles()
        {
            if (panelStyle != null)
            {
                return;
            }

            overlayTexture = CreateTexture(new Color(0.06f, 0.08f, 0.07f, 0.76f));
            panelTexture = CreateTexture(new Color(0.12f, 0.16f, 0.14f, 0.98f));
            primaryButtonTexture = CreateTexture(new Color(0.25f, 0.43f, 0.32f));
            primaryHoverTexture = CreateTexture(new Color(0.34f, 0.53f, 0.40f));
            secondaryButtonTexture = CreateTexture(new Color(0.20f, 0.25f, 0.22f));
            secondaryHoverTexture = CreateTexture(new Color(0.29f, 0.34f, 0.29f));
            accentTexture = CreateTexture(new Color(0.78f, 0.60f, 0.33f));

            panelStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { background = panelTexture },
                border = new RectOffset(2, 2, 2, 2)
            };

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 42,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.94f, 0.88f, 0.75f) }
            };

            subtitleStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.82f, 0.69f, 0.48f) }
            };

            buttonStyle = CreateButtonStyle(primaryButtonTexture, primaryHoverTexture, 21);
            secondaryButtonStyle = CreateButtonStyle(secondaryButtonTexture, secondaryHoverTexture, 17);

            controlsStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 17,
                wordWrap = true,
                normal = { textColor = new Color(0.91f, 0.90f, 0.84f) }
            };

            footerStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.68f, 0.70f, 0.62f) }
            };
        }

        private GUIStyle CreateButtonStyle(Texture2D normalTexture, Texture2D hoverTexture, int fontSize)
        {
            return new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = fontSize,
                fontStyle = FontStyle.Bold,
                normal =
                {
                    background = normalTexture,
                    textColor = new Color(0.97f, 0.95f, 0.88f)
                },
                hover =
                {
                    background = hoverTexture,
                    textColor = Color.white
                },
                active =
                {
                    background = hoverTexture,
                    textColor = Color.white
                }
            };
        }

        private Texture2D CreateTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            texture.hideFlags = HideFlags.HideAndDontSave;
            return texture;
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;

            DestroyTexture(overlayTexture);
            DestroyTexture(panelTexture);
            DestroyTexture(primaryButtonTexture);
            DestroyTexture(primaryHoverTexture);
            DestroyTexture(secondaryButtonTexture);
            DestroyTexture(secondaryHoverTexture);
            DestroyTexture(accentTexture);
        }

        private void DestroyTexture(Texture2D texture)
        {
            if (texture != null)
            {
                Destroy(texture);
            }
        }
    }
}
