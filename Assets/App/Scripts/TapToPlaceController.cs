using Microsoft.MixedReality.Toolkit.Input;
using MRTKExtensions.Utilities;
using TMPro;
using UnityEngine;

public class TapToPlaceController : BaseInputHandler, IMixedRealityPointerHandler
{
    [SerializeField]
    private GameObject _instructionText;

    [SerializeField]
    private float _maxDistance = 3;

    [SerializeField]
    private GameObject _objectToPlace;

    [SerializeField]
    private GameObject _container;

    private TextMeshPro _instructionTextMesh;

    protected override void Start()
    {
        base.Start();

        _instructionTextMesh = _instructionText.GetComponentInChildren<TextMeshPro>();

        _lookAtSurfaceText = $"Please look at the spatial map max {_maxDistance}m ahead of you";
        InputSystem?.RegisterHandler<IMixedRealityPointerHandler>(this);

    }

    private string _lookAtSurfaceText;


    protected override void Update()
    {
        _instructionTextMesh.text =
            LookingDirectionHelpers.GetPositionOnSpatialMap(_maxDistance) != null ?
            "Tap to select a location" : _lookAtSurfaceText;
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
#if !UNITY_EDITOR
        var foundPosition = LookingDirectionHelpers.GetPositionOnSpatialMap(_maxDistance);
#else
        Vector3? foundPosition = LookingDirectionHelpers.GetPositionInLookingDirection(_maxDistance);
#endif
        if (foundPosition != null)
        {
            Instantiate(_objectToPlace, foundPosition.Value, Quaternion.identity, _container.transform);
        }
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
    }

    protected override void RegisterHandlers()
    {
    }

    protected override void UnregisterHandlers()
    {
    }
}
