
public enum EGameSituation
{
//    Loading           =-4,
    InitShowContorl   = -3,
    ShowOne           = -2,
    ShowTwo           = -1,
    None           = 0,
    Opening        = 1, // �}�y.
    JumpBall       = 2,
    AttackA        = 3,
    AttackB        = 4,
    TeeAPicking    = 5, // A ��(���a) �߲y.
    TeeA           = 6, // A ��(���a) ��ɵo�y.
    TeeBPicking    = 7, // B ��(�q��) �߲y.
    TeeB           = 8, // B ��(�q��) ��ɵo�y.
    End            = 9, // ���ɵ���.
    SpecialAction  = 10 // �y���S��t�X, �y���o����|�i�즹���A. ��p�� Jason Terry ����i�}���������ʧ@.
}