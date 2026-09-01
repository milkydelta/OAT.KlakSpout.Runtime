// KlakSpout - Spout video frame sharing plugin for Unity
// https://github.com/keijiro/KlakSpout

using UnityEngine;
using Klak.Spout;

namespace OnAirTap.Spout
{
    [ExecuteInEditMode]
    [AddComponentMenu("Klak/Spout/Spout Sender")]
    public sealed class SpoutSender : MonoBehaviour
    {

        [SerializeField] RenderTexture _sourceTexture;

        public RenderTexture sourceTexture {
            get { return _sourceTexture; }
            set { _sourceTexture = value; }
        }

        [SerializeField] Material _mat;

        public Material material {
            get { return _mat; }
            set { _mat = value; }
        }

        [SerializeField] bool _vflip;

        public bool verticalFlip {
            get { return _vflip; }
            set { _vflip = value; }
        }

        string _spoutName = ("OATKlak" + System.Guid.NewGuid().ToString()).Substring(0, 30);

        public string spoutName
        {
            get { return _spoutName; }
            set { _spoutName = value;DestroyTextureAndSender();}
        }


        #region Private members

        System.IntPtr _plugin;
        Texture2D _sharedTexture;

        #endregion

        void DestroyTextureAndSender()
        {
            if (_plugin != System.IntPtr.Zero)
            {
                Util.IssuePluginEvent(PluginEntry.Event.Dispose, _plugin);
                _plugin = System.IntPtr.Zero;
            }

            Util.Destroy(_sharedTexture);
        }

        public void CaptureFrame()
        {
            if (_sourceTexture == null){return;}

            // Plugin lazy initialization
            if (_plugin == System.IntPtr.Zero)
            {
                _plugin = PluginEntry.CreateSender(name, _sourceTexture.width, _sourceTexture.height);
                if (_plugin == System.IntPtr.Zero) return; // Spout may not be ready.
            }

            // Shared texture lazy initialization
            if (_sharedTexture == null)
            {
                var ptr = PluginEntry.GetTexturePointer(_plugin);
                if (ptr != System.IntPtr.Zero)
                {
                    _sharedTexture = Texture2D.CreateExternalTexture(
                        PluginEntry.GetTextureWidth(_plugin),
                        PluginEntry.GetTextureHeight(_plugin),
                        TextureFormat.ARGB32, false, false, ptr
                    );
                    _sharedTexture.hideFlags = HideFlags.DontSave;
                }
            }

            // Shared texture update
            if (_sharedTexture != null)
            {

                if (_sharedTexture.width != _sourceTexture.width || _sharedTexture.height != _sourceTexture.height)
                {
                    DestroyTextureAndSender();
                    return;
                }
                
                // We can't directly blit to the shared texture (as it lacks
                // render buffer functionality), so we temporarily allocate a
                // render texture as a middleman, blit the source to it, then
                // copy it to the shared texture using the CopyTexture API.
                var tempRT = RenderTexture.GetTemporary(_sharedTexture.width, _sharedTexture.height);

                if (_mat != null)
                {
                    Graphics.Blit(_sourceTexture, tempRT, _mat);
                }
                else if (_vflip)
                {
                    Graphics.Blit(_sourceTexture, tempRT, new Vector2(1.0f, -1.0f), new Vector2(0.0f, 1.0f));
                }
                else
                {
                    Graphics.Blit(_sourceTexture, tempRT);
                }

                Graphics.CopyTexture(tempRT, _sharedTexture);
                RenderTexture.ReleaseTemporary(tempRT);

            }

        }

        #region MonoBehaviour implementation

        void Update()
        {
            // Update the plugin internal state.
            if (_plugin != System.IntPtr.Zero)
                Util.IssuePluginEvent(PluginEntry.Event.Update, _plugin);
        }

        #endregion
    }
}
