using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendSlot : PoolableUIObject
{
    public ParticleSystem PS_Online;
    public ParticleSystem PS_OnlineClose;
    public ParticleSystem PS_Offline;
    public ParticleSystem PS_Selected;

    public ParticleSystem PS_OnlineFalloff;
    public ParticleSystem PS_OnlineCloseFalloff;

    public float t;
    public Vector2 OpacityInTRange;
    public Vector2 OpacityOutTRange;
    public float LifeTime;

    private Color _originalColor;
    private Vector3 _positionOnCurve = Vector3.zero;
    private Vector3 _animationVel = Vector3.zero;

    public void Init(ref Vector3 startPos, float startT, SocialDataType.NetworkMode networkdMode, string name)
    {
        gameObject.transform.position = startPos;
        t = startT;
        CurrentSlotMode = SocialDataType.SlotMode.Navigating;
        CurrentNetworkMode = networkdMode;
        Rend.material.SetFloat("_UIOpacity", 0f);
        Color c = TextRend.color;
        c.a = 0f;
        TextRend.text = name;
        TextRend.color = c;
        _originalColor = PS_Online.GetComponent<ParticleSystemRenderer>().material.GetColor("_TintColor");

        switch (CurrentNetworkMode)
        {
            case SocialDataType.NetworkMode.Online:
                PS_Online.gameObject.SetActive(true);
                break;

            case SocialDataType.NetworkMode.OnlineClose:
                PS_OnlineClose.gameObject.SetActive(true);
                break;

            case SocialDataType.NetworkMode.Offline:
                PS_Offline.gameObject.SetActive(true);
                break;
        }

        _initiated = true;
    }

    private Vector3 GetTargetPosition()
    {
        if(CurrentSlotMode == SocialDataType.SlotMode.Selected)
        {
            float radiusX = GlobalSpiralMaster.GetCurrentRadius(GlobalFriendListManager.SelectedTValue, ref GlobalSpiralMaster.RadiusMapX);
            float radiusY = GlobalSpiralMaster.GetCurrentRadius(GlobalFriendListManager.SelectedTValue, ref GlobalSpiralMaster.RadiusMapY);

            SpiralMath.GetPositionAt(GlobalFriendListManager.SelectedTValue, ref _positionOnCurve, radiusX, radiusY, GlobalSpiralMaster.ScaleZ, GlobalSpiralMaster.NoiseOn);
            return _positionOnCurve + GlobalSpiralMaster.transform.position;
        }
        else
        {
            float radiusX = GlobalSpiralMaster.GetCurrentRadius(t, ref GlobalSpiralMaster.RadiusMapX);
            float radiusY = GlobalSpiralMaster.GetCurrentRadius(t, ref GlobalSpiralMaster.RadiusMapY);

            SpiralMath.GetPositionAt(t, ref _positionOnCurve, radiusX, radiusY, GlobalSpiralMaster.ScaleZ, GlobalSpiralMaster.NoiseOn);
            return _positionOnCurve + GlobalSpiralMaster.transform.position;
        }
    }

    public override void Deactivate()
    {
        GlobalFriendListManager.RemoveFriendFromSlot(this, true);
        base.Deactivate();
    }

    // destroy all the non-animation required friend slot to be destroyed at once
    public void OnFriendlistExit()
    {
        if (t > LifeTime)
        {
            Deactivate();
        }
    }

    private bool IsInSelectedZone()
    {
        float minSelectedT = GlobalFriendListManager.SelectedTValue - GlobalFriendListManager.SelectedTLockInterval;
        float maxSelectedT = GlobalFriendListManager.SelectedTValue + GlobalFriendListManager.SelectedTLockInterval;
        return (t > minSelectedT && t < maxSelectedT);
    }

    // Update is called once per frame
    void Update()
    {
        // Is it initiated?
        if (!_initiated)
        {
            return;
        }

        /*  
         * Let it destroy it self when its not in friend list zone
         * TODO:    have to also keep tracking on t > LifeTime so it is out of 
         *          both spiral end but for TGC UX programming test, skipping this part
        */
        if (t < 0f)
        {
            Deactivate();
        }

        // Get target position per different slot mode and lerp it
        if (IsInSelectedZone())
        {
            if (CurrentSlotMode != SocialDataType.SlotMode.Selected)
            {
                //Handheld.Vibrate(); // vibrate when the slot got into SelecteZone for the first
            }
            CurrentSlotMode = SocialDataType.SlotMode.Selected;
        }
        else
        {
            CurrentSlotMode = SocialDataType.SlotMode.Navigating;
        }

        // set position for slot
        Vector3 targetPosition = GetTargetPosition();
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _animationVel, 0.2f);

        // set visual for slot
        UpdateVisual();
	}

    private void UpdateVisual()
    {
        if (CurrentSlotMode == SocialDataType.SlotMode.Selected)
        {
            ActivateSelectedParticle();
            SetSlotVisual(1.5f, 1.5f, Color.white * 0.5f, Color.white * 0.5f, Color.gray * 0.1f);
        }
        else // when node is not selected
        {
            DeactivateSelectedParticle();
            SetSlotVisual(1f, 0.68f, _originalColor, _originalColor, _originalColor);

            // Animate the color/alpha for the slot along with curve movement
            UpdateOpacityOnMovement();
        }
    }

    private void UpdateOpacityOnMovement()
    {
        if (t <= OpacityInTRange.y)
        {
            float k = SpiralMath.Remap(t, OpacityInTRange.x, OpacityInTRange.y, 0f, 1f);
            SetOpacityForSlot(k);
        }
        else if (t >= OpacityOutTRange.x)
        {
            float k = SpiralMath.Remap(t, OpacityOutTRange.y, OpacityOutTRange.x, 0f, 1f);
            SetOpacityForSlot(k);
        }
        else
        {
            float k = 1f;
            SetOpacityForSlot(k);
        }
    }

    private void SetColorForSlot(ParticleSystem ps, ref Color clr)
    {
        ps.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", clr);
        Rend.material.SetColor("_TintColor", clr);
    }

    private void SetOpacityForSlot(float k)
    {
        Rend.material.SetFloat("_UIOpacity", k);

        PS_Online.GetComponent<ParticleSystemRenderer>().material.SetFloat("_UIOpacity", k);
        PS_OnlineCloseFalloff.GetComponent<ParticleSystemRenderer>().material.SetFloat("_UIOpacity", k);
        PS_OnlineFalloff.GetComponent<ParticleSystemRenderer>().material.SetFloat("_UIOpacity", k);

        Color c = TextRend.color;
        float textAlpha = ((CurrentNetworkMode == SocialDataType.NetworkMode.Offline) ? 0.5f : 1f); ;
        c.a = k * textAlpha;
        TextRend.color = c;
    }

    private void SetScaleForSlot(ParticleSystem ps, float scale)
    {
        ps.gameObject.transform.localScale = new Vector3(scale, scale, scale);
    }

    private void SetScaleForSlotText(float textScale)
    {
        TextRend.gameObject.transform.localScale = new Vector3(textScale, textScale, textScale);
    }

    private void SetSlotVisual(float scale, float textScale, Color onlineColor, Color onlineCloseColor, Color offlineColor)
    {
        switch (CurrentNetworkMode)
        {
            case SocialDataType.NetworkMode.Online:
                SetColorForSlot(PS_Online, ref onlineColor);
                SetScaleForSlot(PS_Online, scale);
                break;

            case SocialDataType.NetworkMode.OnlineClose:
                SetColorForSlot(PS_OnlineClose, ref onlineCloseColor);
                SetScaleForSlot(PS_OnlineClose, scale);
                break;

            case SocialDataType.NetworkMode.Offline:
                SetColorForSlot(PS_Offline, ref offlineColor);
                SetScaleForSlot(PS_Offline, scale);
                break;

            default:
                break;
        }

        SetScaleForSlotText(textScale);
    }

    private void ActivateSelectedParticle()
    {
        if (!PS_Selected.isPlaying)
        {
            PS_Selected.gameObject.SetActive(true);
            PS_Selected.Play();
        }
    }

    private void DeactivateSelectedParticle()
    {
        if (PS_Selected.isPlaying)
        {
            PS_Selected.Stop();
            PS_Selected.gameObject.SetActive(false);
        }
    }
}
