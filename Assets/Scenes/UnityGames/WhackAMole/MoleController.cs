using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class MoleController : MonoBehaviour
{
    [SerializeField] GameObject hitEffectPrefab;
    Transform myT;
    bool myIsAnimating = true;
    private void Start()
    {
        myT = transform;
        this.OnMouseDownAsObservable().
            ThrottleFirst(TimeSpan.FromMilliseconds(2000)) //2秒無視
            .Subscribe(_ =>
            {
                if (WhackAMoleValueManager.time.Value <= 0)
                    return;

                Instantiate(hitEffectPrefab, myT.position, Quaternion.identity);
                myIsAnimating = false;
                ++WhackAMoleValueManager.score.Value;
                Debug.Log("aaaa");
            });
    }
    private async void OnEnable()
    {
        myIsAnimating = true;
        await UniTask.WaitUntil(() => myT != null);

        var moleMoveSequence = DOTween.Sequence()
        .Append(myT.DOMoveY(0, MoleCreater.moleSpawnAndMoveTime).SetEase(Ease.OutSine))
        .AppendInterval(0.3f)
        .SetLoops(2, LoopType.Yoyo)
        .OnComplete(() => gameObject.SetActive(false)); //叩かれなければここで停止なはず

        await UniTask.WaitUntil(() => !myIsAnimating); //叩かれてboolが変わったら下へ
        moleMoveSequence.Pause();

        var moleReturnSequence = DOTween.Sequence()
        .Append(myT.DOMoveY(MoleCreater.moleYPos, 1f).SetEase(Ease.OutSine))
        .OnComplete(() => gameObject.SetActive(false));
    }
}
