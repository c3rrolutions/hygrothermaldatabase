import BaseFilterSubform from "./BaseFilterSubform";
import { ListFilterDefinition, ListPropositionInput } from "../../lib/filter";
import FilterSubform from "./FilterSubform";
import { FilterTypeMap } from "../../lib/filter";

export default function ListFilterSubform<
  TItemPropositionInput,
  TListPropositionInput extends ListPropositionInput<TItemPropositionInput>,
>({
  name,
  ancestors,
  definition,
}: {
  name: readonly (string | number)[];
  ancestors: readonly (string | number)[];
  definition: ListFilterDefinition<TListPropositionInput, keyof TListPropositionInput>;
}) {
  const renderItem = (props: { name: (string | number)[] }) => (
    <FilterSubform
      name={props.name}
      ancestors={ancestors}
      definition={definition.item}
    />
  );
  return (
    <BaseFilterSubform<TListPropositionInput>
      name={name}
      ancestors={ancestors}
      operators={FilterTypeMap["list"].operators}
      initialOperator={FilterTypeMap["list"].initialOperator}
      renderSingleFormItem={renderItem}
      renderMultipleFormItem={renderItem}
    />
  );
}
