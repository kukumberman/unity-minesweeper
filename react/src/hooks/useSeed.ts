import useAppContext from "./useAppContext";

export default function useSeed() {
  const { seed, setSeed } = useAppContext();

  return {
    seed,
    setSeed,
  };
}
