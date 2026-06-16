import { Form, Select, Space } from "antd";
import React from "react";
import {
  getFilterOperatorLabel,
  isFilterOperatorSingle,
  FilterOperator,
} from "../../lib/filter";

type NarrowedOperator<TPropositionInput> = FilterOperator & keyof TPropositionInput;

export default function BaseFilterSubform<TPropositionInput>({
  name,
  ancestors,
  operators,
  renderSingleFormItem,
  renderMultipleFormItem,
  initialOperator,
}: {
  _type?: TPropositionInput;
  name: readonly (string | number)[];
  ancestors: readonly (string | number)[];
  operators: readonly NarrowedOperator<TPropositionInput>[];
  renderSingleFormItem: (props: {
    name: (string | number)[];
  }) => React.ReactNode;
  renderMultipleFormItem: (props: {
    name: (string | number)[];
  }) => React.ReactNode;
  initialOperator: number;
}) {
  const form = Form.useFormInstance();
  const currentOperator =
    (Form.useWatch(
      [...ancestors, ...name, "operator"],
      form,
    ) as NarrowedOperator<TPropositionInput>) ?? operators[initialOperator];

  return (
    <Space.Compact style={{ flex: 1 }}>
      <Form.Item name={[...name, "operator"]} noStyle>
        <Select
          popupMatchSelectWidth={false}
          options={operators.map((key) => ({
            value: key,
            label: getFilterOperatorLabel(key),
          }))}
        />
      </Form.Item>
      <Form.Item noStyle style={{ flex: 1 }}>
        {isFilterOperatorSingle(currentOperator)
          ? renderSingleFormItem({ name: [...name, "value"] })
          : renderMultipleFormItem({ name: [...name, "value"] })}
      </Form.Item>
    </Space.Compact>
  );
}
