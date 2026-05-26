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
        // make it a slider
        [Range(0.0f, 255.0f)]
        public float FadeAlpha = 255f;

        public bool IsRecursive = true;

        SpriteRenderer spr_render;
        Image ui_render;
        TMP_Text text_render;

        List<SpriteRenderer> spr_renders;
        List<Image> ui_renders;
        List<TMP_Text> text_renders;

        /** START **/

        void Start()
        {
            // Get components of main object
            spr_render = GetComponent<SpriteRenderer>();
            ui_render = GetComponent<Image>();
            text_render = GetComponent<TMP_Text>();

            // Prep
            spr_renders = new List<SpriteRenderer>();
            ui_renders = new List<Image>();
            text_renders = new List<TMP_Text>();

            // Get every child of the object
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i);
            }

            // if we are doing a recursive fade, use the childs too! Get their renderers.
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
                t.a = FadeAlpha / 255; /// convert 0-255 to 0-1
                rend.color = t;
            }
        }

        static void setUiRenderAlpha(Image rend, float FadeAlpha)
        {
            if (rend != null)
            {
                Color t = rend.color;
                t.a = FadeAlpha / 255; // convert 0-255 to 0-1
                rend.color = t;
            }
        }

        static void setTextRenderAlpha(TMP_Text rend, float FadeAlpha)
        {
            if (rend != null)
            {
                Color t = rend.color;
                t.a = FadeAlpha / 255; // convert 0-255 to 0-1
                rend.color = t;
            }
        }

        void Update()
        {
            // Limit alpha
            if (FadeAlpha < 0)
            {
                FadeAlpha = 0;
            }
            if (FadeAlpha > 255)
            {
                FadeAlpha = 255;
            }

            // Set alpha for the renderers
            setSprRenderAlpha(spr_render, FadeAlpha);
            setUiRenderAlpha(ui_render, FadeAlpha);
            setTextRenderAlpha(text_render, FadeAlpha);

            // For recursive
            if (IsRecursive)
            {
                // Set renderer alphas for the children
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

            // Custom override function for update (as it's already taken by this class)
            DuringFade();
        }

        public virtual void DuringFade()
        {

        }

        // when starting to implement this, I then thought what is the point
        void OnDestroy()
        {
            if (IsRecursive)
            {

            }
        }
    }
}
