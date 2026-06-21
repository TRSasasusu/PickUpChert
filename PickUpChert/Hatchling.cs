using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PickUpChert {
    public class Hatchling : MonoBehaviour {
        public bool IsInsideShipBeamVolume { get; private set; }

        public void CompleteEnteringShip() {
            IsInsideShipBeamVolume = true;
        }

        public void CompleteExitingShipBeamVolume() {
            IsInsideShipBeamVolume = false;
        }
    }
}
