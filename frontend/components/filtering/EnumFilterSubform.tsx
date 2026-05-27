import { Form } from "antd";
import BaseFilterSubform from "./BaseFilterSubform";
import { EnumPropositionInput } from "../../lib/filter";
import { FilterTypeMap } from "../../lib/filter";
import EnumSelect from "../EnumSelect";

export default function EnumFilterSubform<
  TPropositionInput extends EnumPropositionInput<TEnum>,
  TEnum extends Record<string, TEnumValue>,
  TEnumValue extends string | number,
>({
  name,
  ancestors,
  enumObject,
}: {
  name: readonly (string | number)[];
  ancestors: readonly (string | number)[];
  enumObject: TEnum;
}) {
  return (
    <BaseFilterSubform<TPropositionInput>
      name={name}
      ancestors={ancestors}
      operators={FilterTypeMap["enum"].operators}
      initialOperator={FilterTypeMap["enum"].initialOperator}
      renderSingleFormItem={(props) => (
        <Form.Item
          name={props.name}
          noStyle
          rules={[
            {
              required: true,
            },
          ]}
        >
          <EnumSelect enumObject={enumObject} style={{ width: "100%" }} />
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
          <EnumSelect
            enumObject={enumObject}
            mode="multiple"
            style={{ width: "100%" }}
          />
        </Form.Item>
      )}
    />
  );
}
