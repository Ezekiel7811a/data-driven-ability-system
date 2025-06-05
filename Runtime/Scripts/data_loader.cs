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
        public static IEnumerator LoadAbilitiesFromJson<T>(
            string filename,
            Action<List<IAbility>> onComplete) where T : IAbility, new()
        {
            string path = System.IO.Path.Combine(Application.streamingAssetsPath, filename);
            UnityWebRequest www = UnityWebRequest.Get(path);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error loading abilities: {www.error}");
                onComplete?.Invoke(new List<IAbility>());
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
                    .Select(ConvertToAbility<T>)
                    .ToList();
                onComplete?.Invoke(abilities.Cast<IAbility>().ToList());
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error parsing abilities JSON: {ex}");
                onComplete?.Invoke(new List<IAbility>());
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

        private static T ConvertToAbility<T>(AbilityDTO dto) where T : IAbility, new()
        {
            // T must have a public parameterless ctor
            var ability = new T();
            ability.Name = dto.Name;
            ability.Power = dto.Power;
            ability.Effects = dto.Effects
              .Select(ConvertToContextEffect)
              .ToList();
            return ability;
        }
    }
}