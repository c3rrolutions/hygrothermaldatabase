import { Form, Select } from "antd";
import BaseFilterSubform from "./BaseFilterSubform";
import UuidFormItem from "../UuidFormItem";
import { UuidPropositionInput } from "../../__generated__/graphql";
import { FilterTypeMap } from "../../lib/filter";

export default function UuidFilterSubform<
  TPropositionInput extends UuidPropositionInput,
>({
  name,
  ancestors,
}: {
  name: readonly (string | number)[];
  ancestors: readonly (string | number)[];
}) {
  return (
    <BaseFilterSubform<TPropositionInput>
      name={name}
      ancestors={ancestors}
      operators={FilterTypeMap["uuid"].operators}
      initialOperator={FilterTypeMap["uuid"].initialOperator}
      renderSingleFormItem={(props) => (
        <UuidFormItem name={props.name} style={{ width: "100%" }} />
      )}
      renderMultipleFormItem={(props) => (
        <Form.Item
          name={props.name}
          noStyle
          rules={[
            {
              required: true,
            },
          ]}
        >
          <Select mode="tags" style={{ width: "100%" }} />
        </Form.Item>
      )}
    />
  );
}
