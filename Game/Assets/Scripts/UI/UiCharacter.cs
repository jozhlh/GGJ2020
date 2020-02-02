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

    [SerializeField]
    private SkinnedMeshRenderer m_rend = null;

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
    public void Join( Color color )
    {
        if (m_rend)
        {
            var mat = m_rend.material;
            var colr = mat.GetColor("Fresnel_Colour");
            Debug.Log($"OG: {colr.b}");
            mat.SetColor("Fresnel_Colour", color);
            m_rend.material =mat;
        }

        StartCoroutine(JoinRoutine());
    }

    // Update is called once per frame
    public void ChangeHead( GameObject head, Color color )
    {
        Destroy(m_currentHead);
        m_currentHead = Instantiate( head, m_headAttachPoint, false );
        m_currentHead.name = "Helmet_Temp";
        m_currentHead.transform.localPosition = Vector3.zero;
        m_currentHead.transform.localRotation = Quaternion.identity;

        if (m_rend)
        {
            var mat = m_rend.material;
            var colr = mat.GetColor("Fresnel_Colour");
            Debug.Log($"OG: {colr}");
            mat.SetColor("Fresnel_Colour", color);
            m_rend.material =mat;
        }
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
