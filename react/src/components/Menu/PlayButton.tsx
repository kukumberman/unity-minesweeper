type PlayButtonProps = {
  text: string;
  onClick: () => void;
};

export default function PlayButton(props: PlayButtonProps) {
  return (
    <button
      name="btn-play"
      onClick={props.onClick}
      style={{
        minWidth: 300,
        height: 150,
        fontSize: 69,
        borderRadius: 15,
        padding: "0 50",
      }}
      className="btn bg-success-hoverable text-white-hoverable"
    >
      <text
        style={{
          height: "100%",
        }}
      >
        {props.text}
      </text>
    </button>
  );
}
