using System;
using System.Collections;
using UnityEngine;

public class DashLogic : ActionLogic
{
    /// <summary>
    /// Use CureentTime
    /// </summary>
    /// <param name="baseAction"></param>
    public DashLogic(BaseAction baseAction)
        : base(baseAction) { }

    /// <summary>
    /// This use CurrentTime.
    /// </summary>
    /// <param name="p_dir"></param>
    /// <param name="p_spped"></param>
    /// <param name="p_duration"></param>
    /// <param name="p_easing"></param>
    /// <returns></returns>
    public IEnumerator Dash(
        Vector2 p_dir,
        float p_spped,
        float p_duration,
        Func<float, float> p_easing
    )
    {
        baseAction.GetActionField<ActionFloatField>(ActionFieldName.CurrentTime).value = 0;
        p_dir = p_dir.normalized * p_spped;
        while (
            baseAction.GetActionField<ActionFloatField>(ActionFieldName.CurrentTime).value
            < p_duration
        )
        {
            baseAction.transform.AddPos(
                p_dir
                    * p_easing(
                        1
                            - baseAction
                                .GetActionField<ActionFloatField>(ActionFieldName.CurrentTime)
                                .value / p_duration
                    )
            );

            yield return new WaitForSeconds(Time.fixedDeltaTime);
            baseAction.GetActionField<ActionFloatField>(ActionFieldName.CurrentTime).value +=
                Time.fixedDeltaTime;
        }
    }
}
