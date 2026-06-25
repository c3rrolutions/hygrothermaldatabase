import { Input, Select, Form } from "antd";
import BaseFilterSubform from "./BaseFilterSubform";
import { StringPropositionInput } from "../../__generated__/graphql";
import { FilterTypeMap } from "../../lib/filter";

export default function StringFilterSubform<
  TPropositionInput extends StringPropositionInput,
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
      operators={FilterTypeMap["string"].operators}
      initialOperator={FilterTypeMap["string"].initialOperator}
      renderSingleFormItem={(props) => (
        <Form.Item
          name={props.name}
          noStyle
          rules={[
            {
              required: true,
            },
            {
              whitespace: true,
            },
          ]}
        >
          <Input allowClear style={{ width: "100%" }} />
        </Form.Item>
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
