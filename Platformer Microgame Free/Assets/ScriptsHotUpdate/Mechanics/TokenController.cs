using UnityEngine;
using CSharpLike;
using System.Collections.Generic;

namespace PlatformerMicrogame
{
    /// <summary>
    /// This class animates all token instances in a scene.
    /// This allows a single update call to animate hundreds of sprite 
    /// animations.
    /// If the tokens property is empty, it will automatically find and load 
    /// all token instances in the scene at runtime.
    /// </summary>
    public class TokenController : LikeBehaviour
    {
        [Tooltip("Frames per second at which tokens are animated.")]
        public float frameRate = 12;
        [Tooltip("Instances of tokens which are animated. If empty, token instances are found and loaded at runtime.")]
        public TokenInstance[] tokens;

        [ContextMenu("Find All Tokens")]
        void FindAllTokensInScene()
        {
            object[] temps = HotUpdateBehaviour.FindObjectsOfByType(typeof(TokenInstance));
            tokens = new TokenInstance[temps.Length];
            for (int i = 0; i < temps.Length; i++)
            {
                tokens[i] = temps[i] as TokenInstance;
            }
        }

        void Start()
        {
            //if tokens are empty, find all instances.
            //if tokens are not empty, they've been added at editor time.
            if (tokens.Length == 0)
                FindAllTokensInScene();
            //Register all tokens so they can work with this controller.
            for (var i = 0; i < tokens.Length; i++)
            {
                tokens[i].tokenIndex = i;
                tokens[i].controller = this;
            }
        }
        List<Transform> spriteRenderers = new List<Transform>();
        Transform playerTranform;
        /// <summary>
        /// Check distance between player, and show or hide the GameOjbect that with SpriteRenderer component.
        /// Because we don't need to show it if it far way from player.
        /// Or else the game very lag!
        /// </summary>
        void CheckDistance()
        {
            if (playerTranform == null)
            {
                playerTranform = GameController.Model.player.transform;
                SpriteRenderer[] srs = SpriteRenderer.FindObjectsOfType<SpriteRenderer>(true);
                foreach (SpriteRenderer sr in srs)
                {
                    spriteRenderers.Add(sr.transform);
                }
                spriteRenderers.Remove(playerTranform);
            }
            foreach (Transform t in spriteRenderers)
            {
                if (t != null)
                {
                    bool bNew = (t.position - playerTranform.position).magnitude < 7f;
                    if (bNew != t.gameObject.activeSelf)
                    {
                        t.gameObject.SetActive(bNew);
                    }
                }
            }
        }

        void Update(float deltaTime)
        {
            CheckDistance();
            //update all tokens with the next animation frame.
            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                //if token is null, it has been disabled and is no longer animated.
                if (token != null && token.gameObject.activeSelf && token._renderer != null)
                {
                    token._renderer.sprite = token.sprites[token.frame];
                    if (token.collected && token.frame == token.sprites.Length - 1)
                    {
                        token.gameObject.SetActive(false);
                        spriteRenderers.Remove(token.transform);//remove it from the checking list
                        tokens[i] = null;
                    }
                    else
                    {
                        token.frame = (token.frame + 1) % token.sprites.Length;
                    }
                }
            }
        }

    }
}