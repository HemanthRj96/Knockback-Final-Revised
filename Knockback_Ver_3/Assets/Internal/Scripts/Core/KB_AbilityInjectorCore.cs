using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Collections;

namespace Knockback.Core
{
    public class KB_AbilityInjectorCore : MonoBehaviour
    {
        //todo: Commenting

        [SerializeField]
        private _Ability ability;

        private string _id = "";
        public string injectorId { get { return _id; } private set { _id = value; } }


        public string GetUniqueId()
        {
            if (injectorId != "")
                return injectorId;
            else
            {
                const string _glyphs = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                const int _codeSegments = 3;
                const int _segmentLength = 3;
                string _string = "";

                for (int i = 0; i < _codeSegments - 1; i++)
                {
                    for (int j = 0; j < _segmentLength; j++)
                    {
                        _string += _glyphs[Random.Range(0, _glyphs.Length)];
                    }
                    _string += "-";
                }
                for (int j = 0; j < _segmentLength; j++)
                {
                    _string += _glyphs[Random.Range(0, _glyphs.Length)];
                }
                SetUniqueId(_string);

                return _string;
            }
        }

        protected void SetUniqueId(string id)
        {
            if (injectorId != "")
                return;
            injectorId = id;
        }

        public _Ability GetAbility() => ability;
    }
}