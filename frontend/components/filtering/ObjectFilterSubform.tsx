import { Form, Select, Space } from "antd";
import {
  FilterDefinition,
  getInitialFilterSubformValues,
  ObjectFilterDefinition,
} from "../../lib/filter";
import { getLabel } from "../../lib/string";
import FilterSubform from "./FilterSubform";

export default function ObjectFilterSubform<TObjectPropositionInput>({
  name,
  ancestors,
  definition,
}: {
  _type?: TObjectPropositionInput;
  name: readonly (string | number)[];
  ancestors: readonly (string | number)[];
  definition: ObjectFilterDefinition<
    TObjectPropositionInput,
    keyof TObjectPropositionInput
  >;
}) {
  const form = Form.useFormInstance();
  const fullPath = [...ancestors, ...name];
  const currentIndex =
    (Form.useWatch([...fullPath, "index"], form) as number) ?? 0;

  const handleChange = (newIndex: number) => {
    form.setFieldValue(
      [...fullPath, "value"],
      getInitialFilterSubformValues(
        definition.items[newIndex] as FilterDefinition<any>,
      ),
    );
  };

  return (
    <Space.Compact style={{ flex: 1 }}>
      <Form.Item noStyle name={[...name, "index"]} rules={[{ required: true }]}>
        <Select
          popupMatchSelectWidth={false}
          onChange={handleChange}
          options={definition.items.map((value, index) => ({
            value: index,
            label: getLabel(value, "all-upper"),
          }))}
        />
      </Form.Item>
      <Form.Item noStyle style={{ flex: 1 }}>
        <FilterSubform
          name={[...name, "value"]}
          ancestors={ancestors}
          definition={definition.items[currentIndex]}
        />
      </Form.Item>
    </Space.Compact>
  );
}
