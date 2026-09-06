using UnityEngine;
using UnityEngine.Rendering;

using OnAirTap.Klak.Spout;

namespace OnAirTap.Spout
{

    //
    // Spout sender class (main implementation)
    //
    [ExecuteInEditMode]
    [AddComponentMenu("Klak/Spout/Spout Sender")]
    public sealed class SpoutSender
    {
        #region Spout source

        [SerializeField] string _spoutName = "Spout Sender";

        public string spoutName
        {
            get => _spoutName;
            set => ChangeSpoutName(value);
        }

        void ChangeSpoutName(string name)
        {
            // Sender refresh on renaming
            if (_spoutName == name) return;
            _spoutName = name;
            ReleaseSender();
        }

        #endregion

        #region Format option

        bool _vflip;

        public bool verticalFlip
        {
            get => _vflip;
            set => _vflip = value;
        }

        Material _mat;

        public Material material
        {
            get => _mat;
            set => _mat = value;
        }

        #endregion

        #region Capture target

        [SerializeField] Texture _sourceTexture = null;

        public Texture sourceTexture
        {
            get => _sourceTexture;
            set => _sourceTexture = value;
        }

        #endregion

        #region Sender plugin object

        Sender _sender;

        void ReleaseSender()
        {
            _sender?.Dispose();
            _sender = null;
        }

        #endregion

        #region Buffer texture object

        RenderTexture _buffer;

        void PrepareBuffer(int width, int height)
        {
            // If the buffer exists but has wrong dimensions, destroy it first.
            if (_buffer != null &&
                (_buffer.width != width || _buffer.height != height))
            {
                ReleaseSender();
                Utility.Destroy(_buffer);
                _buffer = null;
            }

            // Create a buffer if it hasn't been allocated yet.
            if (_buffer == null && width > 0 && height > 0)
            {
                _buffer = new RenderTexture(width, height, 0);
                _buffer.hideFlags = HideFlags.DontSave;
                _buffer.Create();
            }
        }

        #endregion


        #region Capture coroutine

        public void CaptureFrame()
        {
            if (!enabled) { return; }

            // Texture capture mode
            if (_sourceTexture == null) return;
            PrepareBuffer(_sourceTexture.width, _sourceTexture.height);


            // Because of differences between Unity and DirectX(?) texture coordinates,
            // the spout sender will appear flipped unless it is actually flipped.
            // KlakSpout's Shader would ordinarily do that automatically, so the Blitter
            // class methods are named the opposite of what actually happens.
            if (_mat != null)
            {
                // I can't specify a Material and do the flip.
                Blitter.BlitVFlipWithMaterial(null, _sourceTexture, _buffer, false, _mat);
            }
            else if (_vflip == false)
            {
                // Actually does no flip
                Blitter.BlitVFlip(null, _sourceTexture, _buffer, false);
            }
            else
            {
                // Does a flip
                Blitter.Blit(null, _sourceTexture, _buffer, false);
            }


            // Sender lazy initialization
            if (_sender == null) _sender = new Sender(_spoutName, _buffer);

            // Sender plugin-side update
            _sender.Update();
        }

        #endregion

        #region MonoBehaviour implementation

        bool _enabled = true;

        public bool enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public void OnDisable()
        {
            ReleaseSender();
            PrepareBuffer(0, 0);
        }

        #endregion
    }

} // namespace Klak.Spout
