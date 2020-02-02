using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiCharacter : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve m_easing = null;

    [SerializeField]
    private float m_easeDuration = 1.0f;


    [SerializeField]
    private Transform m_targetTransform = null;

    [SerializeField]
    private Transform m_character = null;

    [SerializeField]
    private Transform m_headAttachPoint = null;

    [SerializeField]
    private GameObject m_currentHead = null;

    private void Start()
    {
        GameEvents.OnRoundStart += IDontNeedYouNoMore;
    }

    private void OnDestroy()
    {
        GameEvents.OnRoundStart -= IDontNeedYouNoMore;
    }

    void IDontNeedYouNoMore()
    {
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    public void Join()
    {
        StartCoroutine(JoinRoutine());
    }

    // Update is called once per frame
    public void ChangeHead( GameObject head )
    {
        Destroy(m_currentHead);
        m_currentHead = Instantiate( head, Vector3.zero, Quaternion.identity, m_headAttachPoint );
    }

    private IEnumerator JoinRoutine()
    {
        var startPos = m_character.position;
        var target = m_targetTransform.position;
        var count = 0.0f;
        var t = 0.0f;

        while (count < m_easeDuration)
        {
            t = m_easing.Evaluate(count / m_easeDuration);
            m_character.position = Vector3.Lerp( startPos, target, t);
            count += Time.deltaTime;

            yield return null;
        }

        m_character.position = target;
    }
}
