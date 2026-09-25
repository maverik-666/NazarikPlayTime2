namespace Project
{
    using DG.Tweening;
    using LudicWorlds;
    using UnityEngine;

    public class SingleAttackStateState : MonoBehaviour, IState, IAttackState
    { 
        [Header("Debug")]
        [SerializeField] 
        private string _id;
        
        private IAttacker _attacker;
        private IStateMachine _stateMachine;

        private Registry<IDamageable> _targets;
        
        private Timer _afterAttackTimer;
        private float _attackDuration;
        private string _afterAttackStateId;
        
        private bool _isOnCooldown;

        public string ID => _id;
        public bool CanAttack => !_isOnCooldown;

        public void Init(string id, IAttacker attacker, Registry<IDamageable> targets, float attackDuration, string afterAttackStateId, IStateMachine stateMachine)
        {
            _id = id;
            _attacker = attacker;
            _targets = targets;
            _attackDuration = attackDuration;
            _afterAttackStateId = afterAttackStateId;
            _stateMachine = stateMachine;

            _afterAttackTimer = new Timer();
        }

        public void Enter()
        {
            for (int i = 0; i < _targets.Count; i++)
            {
                if (_targets.Items[i].CanNotTakeDamage)
                {
                    continue;
                }
                
                _targets.Items[i].TakeDamage(_attacker.Attack);
                break;
            }
            
            _afterAttackTimer.Start(_attackDuration);
            
            _isOnCooldown = true;
            DOTween.Sequence()
                .SetLink(_attacker.gameObject, LinkBehaviour.KillOnDisable)
                .AppendInterval(_attacker.AttackCooldown)
                .AppendCallback(OnCooldownPass);
        }
        
        private void OnCooldownPass()
        {
            _isOnCooldown = false;
        }

        public void UpdateState()
        {
            _afterAttackTimer.Tick(Time.deltaTime);
            if (_afterAttackTimer.IsStopped)
            {
                AfterAttack();
            }
        }
        
        private void AfterAttack()
        {
            _stateMachine.SetState(_afterAttackStateId);
        }

        public void Exit()
        {
            _afterAttackTimer.Reset();
        }

        public void Dispose() { }
    }
}
