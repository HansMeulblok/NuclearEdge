using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRender : MonoBehaviour
{
    private Tween tween = null;
    public ParticleSystem dust;

    public void Jump()
    {
        if (tween != null) tween.Complete();
        tween = transform.DOScaleY(1.3f, 0.2f);
        PlayDust(0.075f, 200);
    }

    public void Land()
    {
        if (tween != null) tween.Complete();
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        tween = transform.DOPunchScale(new Vector3(0.0f, -0.75f, 0.0f), 1.0f, 4, 0.5f);
        PlayDust(0.25f, 100);
    }

    private void PlayDust(float duration, int count)
    {
        var m = dust.main;
        m.duration = duration;
        var e = dust.emission;
        e.rateOverTime = count;
        dust.Play();
    }

}
