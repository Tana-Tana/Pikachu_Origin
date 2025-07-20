
namespace _Game.Scripts.Effect
{
    public class EffectMatching : GameUnit
    {
        public void OnInit(float time)
        {
            Invoke(nameof(OnDespawn), time);
        }

        private void OnDespawn()
        {
            SimplePool.Despawn(this);
        }
    }
}
