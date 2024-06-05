using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MenuTest : MonoBehaviour
{
    public Image _image;
    void Start()
    {
        DOTween.Init();
        //transform.DORotate(new Vector3(360.0f, 360.0f, 0.0f), 5.0f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetRelative().SetEase(Ease.Linear);
        //transform.DOMove(new Vector3(0.0f, 0.0f, -5.0f), 5.0f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        _image.DOFillAmount(1,2);
        //_image.DOFade(0, 2);
    }
}
