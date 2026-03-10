using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Core
{
    public class FadeObject : MonoBehaviour
    {
        [Range(0.0f, 255.0f)]
        public float FadeAlpha = 255f;

        public bool IsRecursive = false;

        SpriteRenderer spr_render;
        Image ui_render;
        TMP_Text text_render;

        List<SpriteRenderer> spr_renders;
        List<Image> ui_renders;
        List<TMP_Text> text_renders;

        /** START **/

        void Start()
        {
            spr_render = GetComponent<SpriteRenderer>();
            ui_render = GetComponent<Image>();
            text_render = GetComponent<TMP_Text>();

            spr_renders = new List<SpriteRenderer>();
            ui_renders = new List<Image>();
            text_renders = new List<TMP_Text>();

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i);
            }

            if (IsRecursive)
            {
                getRenderers(transform);
            }
        }

        void getRenderers(Transform trans)
        {
            for (int i = 0; i < trans.childCount; i++)
            {
                spr_renders.Add(trans.GetChild(i).GetComponent<SpriteRenderer>());
                ui_renders.Add(trans.GetChild(i).GetComponent<Image>());
                text_renders.Add(trans.GetChild(i).GetComponent<TMP_Text>());
                if (trans.GetChild(i).childCount != 0)
                {
                    // unhealthy for stack? yes.
                    // but I can't think of another way :P
                    getRenderers(trans.GetChild(i));
                }
            }
        }

        /** LOOP **/

        static void setSprRenderAlpha(SpriteRenderer rend, float FadeAlpha)
        {
            if (rend != null)
            {
                Color t = rend.color;
                t.a = FadeAlpha / 255;
                rend.color = t;
            }
        }

        static void setUiRenderAlpha(Image rend, float FadeAlpha)
        {
            if (rend != null)
            {
                Color t = rend.color;
                t.a = FadeAlpha / 255;
                rend.color = t;
            }
        }

        static void setTextRenderAlpha(TMP_Text rend, float FadeAlpha)
        {
            if (rend != null)
            {
                Color t = rend.color;
                t.a = FadeAlpha / 255;
                rend.color = t;
            }
        }

        void Update()
        {
            setSprRenderAlpha(spr_render, FadeAlpha);
            setUiRenderAlpha(ui_render, FadeAlpha);
            setTextRenderAlpha(text_render, FadeAlpha);

            if (IsRecursive)
            {
                foreach (SpriteRenderer one_of_spr_renders in spr_renders)
                {
                    setSprRenderAlpha(one_of_spr_renders, FadeAlpha);
                }
                foreach (Image one_of_ui_renders in ui_renders)
                {
                    setUiRenderAlpha(one_of_ui_renders, FadeAlpha);
                }
                foreach (TMP_Text one_of_text_renders in text_renders)
                {
                    setTextRenderAlpha(one_of_text_renders, FadeAlpha);
                }
            }
        }
        void OnDestroy()
        {
            if (IsRecursive)
            {

            }
        }

        /** PUBLIC FUNCS FOR USER **/

        public void SetFadeAlpha(float newAlpha)
        {
            FadeAlpha = newAlpha;
        }

        public float GetFadeAlpha()
        {
            return FadeAlpha;
        }
    }
}