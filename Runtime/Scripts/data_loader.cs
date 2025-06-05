using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace DataDrivenAbilitySystem
{
    [Serializable]
    public class AbilityDTOList
    {
        public List<AbilityDTO> abilities;
    }

    public class AbilityService : MonoBehaviour
    {
        /// <summary>
        /// Coroutine to load abilities from a JSON file in StreamingAssets.
        /// Usage: StartCoroutine(AbilityService.LoadAbilitiesFromJson(
        ///             "abilities.json", abilities => { /* use them */ }));
        /// </summary>
        public static IEnumerator LoadAbilitiesFromJson(
            string filename,
            Action<List<Ability>> onComplete)
        {
            string path = System.IO.Path.Combine(Application.streamingAssetsPath, filename);
            UnityWebRequest www = UnityWebRequest.Get(path);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error loading abilities: {www.error}");
                onComplete?.Invoke(new List<Ability>());
                yield break;
            }

            try
            {
                // Wrap the text in { "abilities": … } if the file only
                // contains an array:
                string json = www.downloadHandler.text;
                if (json.TrimStart().StartsWith("["))
                {
                    json = $"{{\"abilities\":{json}}}";
                }

                var dtoList = JsonUtility.FromJson<AbilityDTOList>(json);
                if (dtoList?.abilities == null)
                    throw new Exception("No abilities found in JSON.");

                var abilities = dtoList.abilities
                    .Select(ConvertToAbility)
                    .ToList();
                onComplete?.Invoke(abilities);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error parsing abilities JSON: {ex}");
                onComplete?.Invoke(new List<Ability>());
            }
        }

        private static (IEffectContext, IAbilityEffect) ConvertToContextEffect(
            EffectDTO effectDTO)
        {
            IEffectContext context;
            switch (effectDTO.Target.ToLowerInvariant())
            {
                case "self":
                    context = new SelfContext();
                    break;
                case "target":
                    context = new TargetContext();
                    break;
                default:
                    throw new ArgumentException(
                        $"Unknown target context: {effectDTO.Target}");
            }

            IAbilityEffect effect;
            switch (effectDTO.Type.ToLowerInvariant())
            {
                case "damage":
                    effect = new DamageEffect();
                    break;
                default:
                    throw new ArgumentException(
                        $"Unknown effect type: {effectDTO.Type}");
            }

            return (context, effect);
        }

        private static Ability ConvertToAbility(AbilityDTO dto)
        {
            var effects = dto.Effects
                .Select(ConvertToContextEffect)
                .ToList();
            return new Ability
            {
                Name = dto.Name,
                Power = dto.Power,
                Effects = effects
            };
        }
    }
}