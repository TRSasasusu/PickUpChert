using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace PickUpChert {
    public class ShortCutChert : MonoBehaviour {
        GameObject _whiteHole;
        Transform _whiteHoleRenderer;
        InteractReceiver _interactReceiver;

        void Start() {
            _interactReceiver = GetComponent<InteractReceiver>();
            _whiteHole = transform.Find("WarpChert").gameObject;
            _whiteHoleRenderer = _whiteHole.transform.Find("WhiteHoleRenderer");
            _whiteHole.SetActive(false);

            _interactReceiver.OnPressInteract += Interact;
        }

        void Interact() {
            _interactReceiver.SetInteractionEnabled(false);
            _whiteHole.SetActive(true);
            _whiteHoleRenderer.localScale = new Vector3(0.001f, 0.001f, 0.001f);
            _whiteHoleRenderer.DOScale(1.5f, 0.3f).SetLink(_whiteHoleRenderer.gameObject).onComplete += () => {
                var itemTool = Locator.GetToolModeSwapper().GetItemCarryTool();
                itemTool.PickUpItemInstantly(ChertItem.Instance);
                StreamingManager.LoadStreamingAssets("hourglasstwins/meshes/characters", 0);

                _whiteHoleRenderer.DOScale(0.001f, 0.3f).SetLink(_whiteHoleRenderer.gameObject).onComplete += () => {
                    _whiteHole.SetActive(false);
                    _interactReceiver.SetInteractionEnabled(true);
                };
            };
        }
    }
}
