using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendSlot : GameBehaviour
{
    public enum SlotMode { Navigating, Selected };
    public enum NetworkMode { Online, OnlineClose, Offline };

    public Renderer Rend;
    public TextMesh TextRend;
    public ParticleSystem PS_Online;
    public ParticleSystem PS_OnlineClose;
    public ParticleSystem PS_Offline;
    public ParticleSystem PS_Selected;

    public float t;
    public Vector2 OpacityInTRange;
    public Vector2 OpacityOutTRange;
    public float LifeTime;
    public SlotMode CurrentSlotMode { private set; get; }
    public NetworkMode CurrentNetworkMode { private set; get; }

    private Color _originalColor;
    private Vector3 _positionOnCurve = Vector3.zero;
    private Vector3 _animationVel = Vector3.zero;
    private bool _initiated = false;

    public void Init(float startT, NetworkMode networkdMode, string name)
    {
        t = startT;
        CurrentSlotMode = SlotMode.Navigating;
        CurrentNetworkMode = networkdMode;
        Rend.material.SetFloat("_UIOpacity", 0f);
        Color c = TextRend.color;
        c.a = 0f;
        TextRend.text = name;
        TextRend.color = c;

        if (CurrentNetworkMode == NetworkMode.Online)
        {
            _originalColor = PS_Online.GetComponent<ParticleSystemRenderer>().material.GetColor("_TintColor");
            PS_Online.gameObject.SetActive(true);
        }
        else if (CurrentNetworkMode == NetworkMode.OnlineClose)
        {
            _originalColor = PS_OnlineClose.GetComponent<ParticleSystemRenderer>().material.GetColor("_TintColor");
            PS_OnlineClose.gameObject.SetActive(true);
        }
        else if (CurrentNetworkMode == NetworkMode.Offline)
        {
            _originalColor = PS_Offline.GetComponent<ParticleSystemRenderer>().material.GetColor("_TintColor");
            PS_Offline.gameObject.SetActive(true);
        }

        _initiated = true;
    }

    private Vector3 GetTargetPosition()
    {
        if(CurrentSlotMode == SlotMode.Selected)
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

    public void Remove()
    {
        GlobalFriendListManager.RemoveFriendFromSlot(this);
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // Is it initiated?
        if (!_initiated)
        {
            return;
        }

        // Let it destroy it self when its not in friend list zone
        // TODO: change this to use pool system for static memory usage and tracking later
        if (t > LifeTime || t < 0f)
        {
            Remove();
        }

        // Get target position per different slot mode and lerp it
        if (t > GlobalFriendListManager.SelectedTValue - GlobalFriendListManager.SelectedTLockInterval &&
            t < GlobalFriendListManager.SelectedTValue + GlobalFriendListManager.SelectedTLockInterval)
        {
            CurrentSlotMode = SlotMode.Selected;
        }
        else
        {
            CurrentSlotMode = SlotMode.Navigating;
        }

        // set position for slot
        Vector3 targetPosition = GetTargetPosition();
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _animationVel, 0.2f);

        // set visual for slot
        UpdateVisual();
	}

    private void UpdateVisual()
    {
        if (CurrentSlotMode == SlotMode.Selected)
        {
            if (!PS_Selected.isPlaying){
                PS_Selected.gameObject.SetActive(true);
                PS_Selected.Play();
            }

            float scale = 1.5f;
            float textScale = 1.5f;
            TextRend.gameObject.transform.localScale = new Vector3(textScale, textScale, textScale);
            if (CurrentNetworkMode == NetworkMode.Online)
            {
                PS_Online.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.white * 0.5f);
                Rend.material.SetColor("_TintColor", Color.white * 0.5f);
                PS_Online.gameObject.transform.localScale = new Vector3(scale, scale, scale);
            }
            else if (CurrentNetworkMode == NetworkMode.OnlineClose)
            {
                PS_OnlineClose.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.white * 0.5f);
                Rend.material.SetColor("_TintColor", Color.white * 0.5f);
                PS_OnlineClose.gameObject.transform.localScale = new Vector3(scale, scale, scale);
            }
            else if (CurrentNetworkMode == NetworkMode.Offline)
            {
                PS_Offline.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.gray * 0.1f);
                Rend.material.SetColor("_TintColor", Color.gray * 0.1f);
                PS_Offline.gameObject.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
        else 
        {
            if (PS_Selected.isPlaying)
            {
                PS_Selected.Stop();
                PS_Selected.gameObject.SetActive(false);
            }

            float scale = 1f;
            float textScale = 0.68f;
            TextRend.gameObject.transform.localScale = new Vector3(textScale, textScale, textScale);
            if (CurrentNetworkMode == NetworkMode.Online)
            {
                PS_Online.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", _originalColor);
                Rend.material.SetColor("_TintColor", _originalColor);
                PS_Online.gameObject.transform.localScale = new Vector3(scale,scale,scale);
            }
            else if (CurrentNetworkMode == NetworkMode.OnlineClose)
            {
                PS_OnlineClose.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", _originalColor);
                Rend.material.SetColor("_TintColor", _originalColor);
                PS_OnlineClose.gameObject.transform.localScale = new Vector3(scale,scale,scale);
            }
            else if (CurrentNetworkMode == NetworkMode.Offline)
            {
                PS_Offline.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", _originalColor);
                Rend.material.SetColor("_TintColor", _originalColor);
                PS_Offline.gameObject.transform.localScale = new Vector3(scale,scale,scale);
            }

            float textAlpha = ((CurrentNetworkMode == NetworkMode.Offline) ? 0.5f : 1f);
            // animate opacity based on location on the curve
            if (t <= OpacityInTRange.y)
            {
                float k = SpiralMath.Remap(t, OpacityInTRange.x, OpacityInTRange.y, 0f, 1f);
                Rend.material.SetFloat("_UIOpacity", k);
                PS_Online.GetComponent<ParticleSystemRenderer>().material.SetFloat("_UIOpacity", k);
                Color c = TextRend.color;
                c.a = k * textAlpha;
                TextRend.color = c;
            }
            else if (t >= OpacityOutTRange.x)
            {
                float k = SpiralMath.Remap(t, OpacityOutTRange.y, OpacityOutTRange.x, 0f, 1f);
                Rend.material.SetFloat("_UIOpacity", k);
                PS_Online.GetComponent<ParticleSystemRenderer>().material.SetFloat("_UIOpacity", k);
                Color c = TextRend.color;
                c.a = k * textAlpha;
                TextRend.color = c;
            }
            else
            {
                float k = 1f;
                Rend.material.SetFloat("_UIOpacity", k);
                PS_Online.GetComponent<ParticleSystemRenderer>().material.SetFloat("_UIOpacity", k);
                Color c = TextRend.color;
                c.a = k * textAlpha;
                TextRend.color = c;
            }
        }
    }
}
