type StageSectionProps = {
  title: string;
  index: number;
  stages: string[];
  onChange: (evt: any) => void;
};

export default function StageSection(props: StageSectionProps) {
  return (
    <div name="container-stage" style={{ marginTop: 50 }}>
      <text
        style={{
          fontSize: 30,
          fontStyle: "bold",
          textAlign: "center",
          verticalAlign: "middle",
        }}
      >
        {props.title}
      </text>
      <radioButtonGroup
        choices={props.stages}
        value={props.index}
        style={{ fontSize: 25 }}
        onChange={props.onChange}
      />
    </div>
  );
}
