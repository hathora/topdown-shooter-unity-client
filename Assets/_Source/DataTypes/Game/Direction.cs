namespace DataTypes.Game {

    // The order of Up and Down is reversed here
    // compared to that of the Server's datatype
    // because Unity's coordinate system is the
    // opposite of Phaser's?

    public enum Direction {
        None,
        Down,
        Up,
        Left,
        Right,
    }
}