// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [ExecuteAlways]
    public class SpherePointerVisual : MonoBehaviour
    {
        public Transform TetherEndPoint => tetherEndPoint;

        public bool TetherVisualsEnabled => tetherVisualsEnabled;

        [Tooltip("The pointer these visuals decorate")]
        private SpherePointer pointer;

        [SerializeField]
        [Tooltip("Tether will not be shown unless it is at least this long")]
        private float minTetherLength = 0.03f;

        [SerializeField]
        private Transform visualsRoot = null;

        [SerializeField]
        private Transform tetherEndPoint = null;

        /// Assumption: Tether line is a child of the visuals!
        [SerializeField]
        private BaseMixedRealityLineDataProvider tetherLine = null;

        private bool tetherVisualsEnabled;

        public void OnEnable()
        {
            CheckInitialization();
        }

        public void OnDestroy()
        {
            if (visualsRoot != null)
            {
                Destroy(visualsRoot.gameObject);
            }
        }

        public void Start()
        {
            // put it at root of scene
            MixedRealityPlayspace.AddChild(visualsRoot.transform);
            visualsRoot.gameObject.name = $"{gameObject.name}_NearTetherVisualsRoot";
        }

        private void CheckInitialization()
        {
            if (pointer == null)
            {
                pointer = GetComponent<SpherePointer>();
            }
            if (pointer == null)
            {
                Debug.LogError($"No SpherePointer found on {gameObject.name}.");
            }

            CheckAsset(visualsRoot, "Visuals Root");
            CheckAsset(tetherEndPoint, "Tether End Point");
            CheckAsset(tetherLine, "Tether Line");
        }

        private void CheckAsset(object asset, string fieldname)
        {
            if (asset == null)
            {
                Debug.LogError($"No {fieldname} specified on {gameObject.name}.SpherePointerVisual. Did you forget to set the {fieldname}?");
            }
        }

        public void Update()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            tetherVisualsEnabled = false;
            if (pointer.IsFocusLocked && pointer.IsTargetPositionLockedOnFocusLock && pointer.Result != null)
            {
                NearInteractionGrabbable grabbedObject = GetGrabbedObject();
                if (grabbedObject != null && grabbedObject.ShowTetherWhenManipulating)
                {
                    Vector3 graspPosition;
                    pointer.TryGetNearGraspPoint(out graspPosition);
                    tetherLine.FirstPoint = graspPosition;
                    Vector3 endPoint = pointer.Result.Details.Object.transform.TransformPoint(pointer.Result.Details.PointLocalSpace);
                    tetherLine.LastPoint = endPoint;
                    tetherVisualsEnabled = Vector3.Distance(tetherLine.FirstPoint, tetherLine.LastPoint) > minTetherLength;
                    tetherLine.enabled = tetherVisualsEnabled;
                    tetherEndPoint.gameObject.SetActive(tetherVisualsEnabled);
                    tetherEndPoint.position = endPoint;
                }
            }

            visualsRoot.gameObject.SetActive(tetherVisualsEnabled);
        }

        private NearInteractionGrabbable GetGrabbedObject()
        {
            if (pointer.Result?.Details.Object != null)
            {
                return pointer.Result.Details.Object.GetComponent<NearInteractionGrabbable>();
            }
            else
            {
                return null;
            }
        }
    }
}