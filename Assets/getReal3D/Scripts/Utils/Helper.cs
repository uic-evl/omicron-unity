using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace getReal3D {
    public class Helper {
        public static void SetActiveRecursively(GameObject go, bool active)
        {
#       if !UNITY_3_5
        go.SetActive(active);
#       else
        go.SetActiveRecursively(active);
#       endif
        }
    }
}
