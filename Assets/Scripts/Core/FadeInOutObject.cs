using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Core
{
    public class FadeInOutObject : FadeObject
    {
        // Fade states
        enum Fade
        {
            IDLE,
            IN,
            OUT
        } Fade state = Fade.IDLE;

        // Default speed
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

        // Read only booleans for public use.
        public bool FadedIn { get { return FadeAlpha >= 255; } }
        public bool FadedOut { get { return FadeAlpha <= 0; } }
        public bool Faded { get { return FadedIn || FadedOut; } }

        public override void DuringFade()
        {
            switch (state)
            {
                // Fading in
                case Fade.IN:
                    {
                        // If we haven't reached to alpha 255
                        if (FadeAlpha < 255)
                        {
                            // Keep on fading in! (based on delta time)
                            FadeAlpha += Time.deltaTime * (100 * speed);
                        }
                        // If we have reached to alpha 255
                        else
                        {
                            // We faded in! Limit it to 255.
                            FadeAlpha = 255;
                            // Wait for another fade in/out request
                            state = Fade.IDLE;
                        }
                        break;
                    }
                // Fading out
                case Fade.OUT:
                    {
                        // If we haven't reached to alpha 0
                        if (FadeAlpha > 0)
                        {
                            // Keep on fading out! (based on delta time)
                            FadeAlpha -= Time.deltaTime * (100 * speed);
                        }
                        // If we have reached to alpha 0
                        else
                        {
                            // We faded out! Wait for another fade in/out request
                            state = Fade.IDLE;
                            // Limit alpha to 0
                            FadeAlpha = 0;
                        }
                        break;
                    }
            }
        }
    }
}
