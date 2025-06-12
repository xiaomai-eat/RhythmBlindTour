using System.Linq;
using UnityEngine;
using Qf.Systems;
using Qf.Models.AudioEdit;
using Qf.ClassDatas.AudioEdit;

public class mInputJudgeController : MonoBehaviour
{
    private CreateDrumsManager drumsManager;
    private AudioEditModel editModel;
    private bool inputConsumedThisFrame = false;

    private void Start()
    {
        drumsManager = FindObjectOfType<CreateDrumsManager>();
        editModel = GameBody.Interface.GetModel<AudioEditModel>();
    }

    private void Update()
    {
        if (drumsManager == null || editModel == null || !editModel.Mode.Equals(SystemModeData.PlayMode))
            return;

        inputConsumedThisFrame = false;

        TryHandle(TheTypeOfOperation.Click, InputSystems.Click);
        TryHandle(TheTypeOfOperation.SwipeUp, InputSystems.SwipeUp);
        TryHandle(TheTypeOfOperation.SwipeDown, InputSystems.SwipeDown);
        TryHandle(TheTypeOfOperation.SwipeLeft, InputSystems.SwipeLeft);
        TryHandle(TheTypeOfOperation.SwipeRight, InputSystems.SwipeRight);

        if (!inputConsumedThisFrame && InputSystems.PlayClick)
        {
            HandleFirstLose();
        }
    }

    void TryHandle(TheTypeOfOperation type, bool inputTriggered)
    {
        if (!inputTriggered || inputConsumedThisFrame)
            return;

        var candidates = drumsManager.ActiveInputModes
            .Where(x => x != null && x.IsActive)
            .OrderBy(x => x.StartTime)
            .ToList();

        var matched = candidates
            .Where(x => x.GetOperation() == type)
            .ToList();

        if (matched.Count > 0)
        {
            var first = matched.First();
            first.SucceedByManager();
            first.IsActive = false;

            inputConsumedThisFrame = true;
        }
    }

    void HandleFirstLose()
    {
        var fallback = drumsManager.ActiveInputModes
            .Where(x => x != null && x.IsActive)
            .OrderBy(x => x.StartTime)
            .ToList();

        if (fallback.Count > 0)
        {
            var first = fallback.First();
            first.LoseByManager();
            first.IsActive = false;

            inputConsumedThisFrame = true;
        }
    }
}
