using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AweEditor.Datatypes
{
    public class TextureMaps
    {
        private string _defaultTexture;
        private Dictionary<int, string> _textureIds;

        public TextureMaps()
        {
            _textureIds = new Dictionary<int, string>();
        }

        public string this[int key]
        {
            get
            {
                if (_textureIds.ContainsKey(key))
                    return _textureIds[key];
                else
                    return _defaultTexture;
            }
            set
            {
                _textureIds[key] = value;
            }
        }

        public bool HasDefaultTexture
        {
            get { return _defaultTexture != null; }
        }

        public void SetDefaultTexture(string textureName)
        {
            _defaultTexture = textureName;
        }

        public void Reset()
        {
            _defaultTexture = null;
            _textureIds.Clear();
        }
    }
}
