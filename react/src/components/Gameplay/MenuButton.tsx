type MenuButtonProps = {
  text: string;
  onClick: () => void;
};

export function MenuButton(props: MenuButtonProps) {
  return (
    <button
      onClick={props.onClick}
      style={{
        position: "absolute",
        left: 50,
        top: 50,
        width: 200,
        height: 100,
        borderRadius: 10,
      }}
      className="btn bg-primary-hoverable text-white-hoverable"
    >
      <text style={{ height: "100%", fontSize: 50 }}>{props.text}</text>
    </button>
  );
}
