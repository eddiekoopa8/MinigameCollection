using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Core
{
    public class FadeInOutObject : FadeObject
    {
        enum Fade
        {
            IDLE,
            IN,
            OUT
        } Fade state = Fade.IDLE;

        float speed = 1.0f;

        public void FadeIn(float FadeSpeed = 1.0f)
        {
            state = Fade.IN;
            speed = FadeSpeed;
        }

        public void FadeOut(float FadeSpeed = 1.0f)
        {
            state = Fade.OUT;
            speed = FadeSpeed;
        }

        public bool FadedIn { get { return FadeAlpha >= 255; } }
        public bool FadedOut { get { return FadeAlpha <= 0; } }
        public bool Faded { get { return FadedIn || FadedOut; } }

        public override void DuringFade()
        {
            switch (state)
            {
                case Fade.IN:
                    {
                        if (FadeAlpha < 255)
                        {
                            FadeAlpha += Time.deltaTime * (100 * speed);
                        }
                        else
                        {
                            FadeAlpha = 255;
                            state = Fade.IDLE;
                        }
                        break;
                    }
                case Fade.OUT:
                    {
                        if (FadeAlpha > 0)
                        {
                            FadeAlpha -= Time.deltaTime * (100 * speed);
                        }
                        else
                        {
                            state = Fade.IDLE;
                            FadeAlpha = 0;
                        }
                        break;
                    }
            }
        }
    }
}