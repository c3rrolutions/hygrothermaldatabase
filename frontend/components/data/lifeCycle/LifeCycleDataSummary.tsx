import paths from "../../../paths";
import { LifeCycleDataPartialFragment } from "../../../queries/data.generated";
import DataSummary from "../DataSummary";

export default function LifeCycleDataSummary({
  entity,
}: {
  entity: LifeCycleDataPartialFragment;
}) {
  return (
    <DataSummary entity={entity} route={paths.lifeCycleData}></DataSummary>
  );
}
