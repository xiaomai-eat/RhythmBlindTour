using UnityEngine;
using Qf.Models.AudioEdit;

[RequireComponent(typeof(InputMode))]
public class mInputModeVisualController : MonoBehaviour
{
    public Transform judgeLineTarget;         // 判定基准线（只用于方向）
    public Transform judgmentBarTransform;    // 用于显示判定区间的物体（横向拉长）

    private InputMode inputMode;
    private AudioEditModel editModel;

    private Vector3 moveDirection;
    private float moveSpeedPerSecond;
    private float distanceToMove;

    void Start()
    {
        inputMode = GetComponent<InputMode>();
        editModel = inputMode.GetArchitecture().GetModel<AudioEditModel>();

        Vector3 targetPos = judgeLineTarget.position;
        Vector3 currentPos = transform.position;

        distanceToMove = targetPos.x - currentPos.x;
        moveDirection = distanceToMove >= 0 ? Vector3.right : Vector3.left;

        // 修正为：鼓点中心时间（centerTime）抵达判定线
        float centerTime = (inputMode.StartTime + inputMode.EndTime) / 2f;
        float timeToReachCenter = centerTime - inputMode.PreAdventTime;
        moveSpeedPerSecond = Mathf.Abs(distanceToMove) / timeToReachCenter;

        // 设置“判定区间”长度（EndTime - StartTime）对应的横向视觉长度
        float judgmentDuration = inputMode.EndTime - inputMode.StartTime;
        float barLength = moveSpeedPerSecond * judgmentDuration;

        if (judgmentBarTransform != null)
        {
            Vector3 scale = judgmentBarTransform.localScale;
            scale.x = barLength;
            judgmentBarTransform.localScale = scale;
        }
    }


    void FixedUpdate()
    {
        if (inputMode == null || inputMode.HasJudged)
            return;

        float deltaMove = moveSpeedPerSecond * Time.fixedDeltaTime;
        transform.position += moveDirection * deltaMove;
    }
}
