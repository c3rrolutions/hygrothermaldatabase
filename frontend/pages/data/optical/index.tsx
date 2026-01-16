import Layout from "../../../components/Layout";
import {
  Table,
  App,
  Form,
  Button,
  Alert,
  Typography,
  Descriptions,
} from "antd";
import { AllOpticalDataDocument } from "../../../queries/data.generated";
import {
  OpticalData,
  Scalars,
  OpticalDataPropositionInput,
} from "../../../__generated__/graphql";
import { useState } from "react";
import { setMapValue } from "../../../lib/freeTextFilter";
import {
  getAppliedMethodColumnProps,
  getComponentUuidColumnProps,
  getDescriptionColumnProps,
  getNameColumnProps,
  getResourceTreeColumnProps,
  getTimestampColumnProps,
  getUuidColumnProps,
} from "../../../lib/table";
import {
  FloatPropositionComparator,
  FloatPropositionFormList,
} from "../../../components/FloatPropositionFormList";
import {
  UuidPropositionComparator,
  UuidPropositionFormList,
} from "../../../components/UuidPropositionFormList";
import paths from "../../../paths";
import { useQuery } from "@apollo/client/react";

const layout = {
  labelCol: { span: 8 },
  wrapperCol: { span: 16 },
};
const tailLayout = {
  wrapperCol: { offset: 8, span: 16 },
};

enum Negator {
  Is = "is",
  IsNot = "isNot",
}

const negateIfNecessary = (
  negator: Negator,
  proposition: OpticalDataPropositionInput,
): OpticalDataPropositionInput => {
  switch (negator) {
    case Negator.Is:
      return proposition;
    case Negator.IsNot:
      // return { not: proposition };
      return proposition; // TODO Support `not` in filter!
  }
};

const conjunct = (
  propositions: OpticalDataPropositionInput[],
): OpticalDataPropositionInput => {
  if (propositions.length == 0) {
    return {};
  }
  if (propositions.length == 1) {
    return propositions[0];
  }
  return { and: propositions };
};

// const disjunct = (
//   propositions: OpticalDataPropositionInput[]
// ): OpticalDataPropositionInput => {
//   if (propositions.length == 0) {
//     return {};
//   }
//   if (propositions.length == 1) {
//     return propositions[0];
//   }
//   return { or: propositions };
// };

function Page() {
  const [form] = Form.useForm();
  const [filtering, setFiltering] = useState(false);
  const [globalErrorMessages, setGlobalErrorMessages] = useState(
    new Array<string>(),
  );
  const { message } = App.useApp();
  const [data, setData] = useState<OpticalData[]>([]);
  // Using `skip` is inspired by https://github.com/apollographql/apollo-client/issues/5268#issuecomment-749501801
  // An alternative would be `useLazy...` as told in https://github.com/apollographql/apollo-client/issues/5268#issuecomment-527727653
  // `useLazy...` does not return a `Promise` though as `use...Query.refetch` does which is used below.
  // For error policies see https://www.apollographql.com/docs/react/v2/data/error-handling/#error-policies
  const allOpticalDataQuery = useQuery(AllOpticalDataDocument, {
    skip: true,
    errorPolicy: "all",
  });

  const [filterText, setFilterText] = useState(() => new Map<string, string>());
  const onFilterTextChange = setMapValue(filterText, setFilterText);

  const onFinish = ({
    componentIds,
    dataFormatIds,
    infraredEmittances,
    nearnormalHemisphericalSolarReflectances,
    nearnormalHemisphericalSolarTransmittances,
    nearnormalHemisphericalVisibleReflectances,
    nearnormalHemisphericalVisibleTransmittances,
  }: {
    componentIds:
      | {
          negator: Negator;
          comparator: UuidPropositionComparator;
          value: Scalars["Uuid"]["input"] | undefined;
        }[]
      | undefined;
    dataFormatIds:
      | {
          negator: Negator;
          comparator: UuidPropositionComparator;
          value: Scalars["Uuid"]["input"] | undefined;
        }[]
      | undefined;
    infraredEmittances:
      | {
          negator: Negator;
          comparator: FloatPropositionComparator;
          value: number | undefined;
        }[]
      | undefined;
    nearnormalHemisphericalSolarReflectances:
      | {
          negator: Negator;
          comparator: FloatPropositionComparator;
          value: number | undefined;
        }[]
      | undefined;
    nearnormalHemisphericalSolarTransmittances:
      | {
          negator: Negator;
          comparator: FloatPropositionComparator;
          value: number | undefined;
        }[]
      | undefined;
    nearnormalHemisphericalVisibleReflectances:
      | {
          negator: Negator;
          comparator: FloatPropositionComparator;
          value: number | undefined;
        }[]
      | undefined;
    nearnormalHemisphericalVisibleTransmittances:
      | {
          negator: Negator;
          comparator: FloatPropositionComparator;
          value: number | undefined;
        }[]
      | undefined;
  }) => {
    const filter = async () => {
      try {
        setFiltering(true);
        // https://www.apollographql.com/docs/react/networking/authentication/#reset-store-on-logout
        const propositions: OpticalDataPropositionInput[] = [];
        if (componentIds) {
          for (let { negator, comparator, value } of componentIds) {
            propositions.push(
              negateIfNecessary(negator, {
                componentId: { [comparator]: value },
              }),
            );
          }
        }
        if (dataFormatIds) {
          for (let { negator, comparator, value } of dataFormatIds) {
            propositions.push(
              negateIfNecessary(negator, {
                resources: {
                  some: {
                    dataFormatId: { [comparator]: value },
                  },
                },
              }),
            );
          }
        }
        // Note that `0` evaluates to `false`, so below we cannot use
        // `if (value)`.
        if (infraredEmittances) {
          for (let { negator, comparator, value } of infraredEmittances) {
            if (value !== undefined && value !== null) {
              propositions.push(
                negateIfNecessary(negator, {
                  infraredEmittances: {
                    some: {
                      [comparator]: value,
                    },
                  },
                }),
              );
            }
          }
        }
        if (nearnormalHemisphericalSolarReflectances) {
          for (let {
            negator,
            comparator,
            value,
          } of nearnormalHemisphericalSolarReflectances) {
            if (value !== undefined && value !== null) {
              propositions.push(
                negateIfNecessary(negator, {
                  nearnormalHemisphericalSolarReflectances: {
                    some: {
                      [comparator]: value,
                    },
                  },
                }),
              );
            }
          }
        }
        if (nearnormalHemisphericalSolarTransmittances) {
          for (let {
            negator,
            comparator,
            value,
          } of nearnormalHemisphericalSolarTransmittances) {
            if (value !== undefined && value !== null) {
              propositions.push(
                negateIfNecessary(negator, {
                  nearnormalHemisphericalSolarTransmittances: {
                    some: {
                      [comparator]: value,
                    },
                  },
                }),
              );
            }
          }
        }
        if (nearnormalHemisphericalVisibleReflectances) {
          for (let {
            negator,
            comparator,
            value,
          } of nearnormalHemisphericalVisibleReflectances) {
            if (value !== undefined && value !== null) {
              propositions.push(
                negateIfNecessary(negator, {
                  nearnormalHemisphericalVisibleReflectances: {
                    some: {
                      [comparator]: value,
                    },
                  },
                }),
              );
            }
          }
        }
        if (nearnormalHemisphericalVisibleTransmittances) {
          for (let {
            negator,
            comparator,
            value,
          } of nearnormalHemisphericalVisibleTransmittances) {
            if (value !== undefined && value !== null) {
              propositions.push(
                negateIfNecessary(negator, {
                  nearnormalHemisphericalVisibleTransmittances: {
                    some: {
                      [comparator]: value,
                    },
                  },
                }),
              );
            }
          }
        }
        const { error, data } = await allOpticalDataQuery.refetch(
          propositions.length == 0
            ? {}
            : {
                where: conjunct(propositions),
              },
        );
        if (error) {
          // TODO Handle properly.
          console.log(error);
          message.error(error.message);
        }
        // TODO Casting to `OpticalData` is wrong and error prone!
        setData(
          (data?.allOpticalData?.edges?.map((x) => x.node) ||
            []) as OpticalData[],
        );
      } catch (error) {
        // TODO Handle properly.
        console.log("Failed:", error);
      } finally {
        setFiltering(false);
      }
    };
    filter();
  };

  const onFinishFailed = () => {
    setGlobalErrorMessages(["Fix the errors below."]);
  };

  return (
    <Layout>
      <Typography.Title>Optical Data</Typography.Title>
      {/* TODO Display error messages in a list? */}
      {globalErrorMessages.length > 0 && (
        <Alert type="error" message={globalErrorMessages.join(" ")} />
      )}
      <Form
        {...layout}
        form={form}
        name="filterData"
        onFinish={onFinish}
        onFinishFailed={onFinishFailed}
      >
        <UuidPropositionFormList name="componentIds" label="Component Id" />
        <UuidPropositionFormList name="dataFormatIds" label="Data Format Id" />
        <FloatPropositionFormList
          name="infraredEmittances"
          label="Infrared emittance"
        />
        <FloatPropositionFormList
          name="nearnormalHemisphericalSolarReflectances"
          label="Nearnormal hemispherical solar reflectance"
        />
        <FloatPropositionFormList
          name="nearnormalHemisphericalSolarTransmittances"
          label="Nearnormal hemispherical solar transmittance"
        />
        <FloatPropositionFormList
          name="nearnormalHemisphericalVisibleReflectances"
          label="Nearnormal hemispherical visible reflectance"
        />
        <FloatPropositionFormList
          name="nearnormalHemisphericalVisibleTransmittances"
          label="Nearnormal hemispherical visible transmittance"
        />

        <Form.Item {...tailLayout}>
          <Button type="primary" htmlType="submit" loading={filtering}>
            Filter
          </Button>
        </Form.Item>
      </Form>
      <Table
        loading={filtering}
        columns={[
          {
            ...getUuidColumnProps<(typeof data)[0]>(
              onFilterTextChange,
              (x) => filterText.get(x),
              paths.opticalDatum,
            ),
          },
          {
            ...getNameColumnProps<(typeof data)[0]>(onFilterTextChange, (x) =>
              filterText.get(x),
            ),
          },
          {
            ...getDescriptionColumnProps<(typeof data)[0]>(
              onFilterTextChange,
              (x) => filterText.get(x),
            ),
          },
          {
            ...getTimestampColumnProps<(typeof data)[0]>(),
          },
          {
            ...getComponentUuidColumnProps<(typeof data)[0]>(
              onFilterTextChange,
              (x) => filterText.get(x),
            ),
          },
          // {
          //   ...getInternallyLinkedFilterableStringColumnProps<typeof data[0]>(
          //     "Database UUID",
          //     "databaseId",
          //     (x) => x.databaseId,
          //     onFilterTextChange,
          //     (x) => filterText.get(x),
          //     (x) => paths.database(x.databaseId)
          //   ),
          // },
          {
            ...getAppliedMethodColumnProps<(typeof data)[0]>(
              onFilterTextChange,
              (x) => filterText.get(x),
            ),
          },
          {
            ...getResourceTreeColumnProps<(typeof data)[0]>(
              onFilterTextChange,
              (x) => filterText.get(x),
            ),
          },
          {
            title: "Emittances, Reflectances, and Transmittances",
            key: "nearnormalHemisphericalX",
            render: (_text, record, _index) => (
              <Descriptions column={1}>
                <Descriptions.Item
                  key="infraredEmittances"
                  label="Infrared Emittances"
                >
                  {record.infraredEmittances
                    .map((x) => x.toLocaleString("en"))
                    .join(", ")}
                </Descriptions.Item>
                <Descriptions.Item
                  key="nearnormalHemisphericalSolarReflectances"
                  label="Nearnormal Hemispherical Solar Reflectances"
                >
                  {record.nearnormalHemisphericalSolarReflectances
                    .map((x) => x.toLocaleString("en"))
                    .join(", ")}
                </Descriptions.Item>
                <Descriptions.Item
                  key="nearnormalHemisphericalSolarTransmittances"
                  label="Nearnormal Hemispherical Solar Transmittances"
                >
                  {record.nearnormalHemisphericalSolarTransmittances
                    .map((x) => x.toLocaleString("en"))
                    .join(", ")}
                </Descriptions.Item>
                <Descriptions.Item
                  key="nearnormalHemisphericalVisibleReflectances"
                  label="Nearnormal Hemispherical Visible Reflectances"
                >
                  {record.nearnormalHemisphericalVisibleReflectances
                    .map((x) => x.toLocaleString("en"))
                    .join(", ")}
                </Descriptions.Item>
                <Descriptions.Item
                  key="nearnormalHemisphericalVisibleTransmittances"
                  label="Nearnormal Hemispherical Visible Transmittances"
                >
                  {record.nearnormalHemisphericalVisibleTransmittances
                    .map((x) => x.toLocaleString("en"))
                    .join(", ")}
                </Descriptions.Item>
              </Descriptions>
            ),
          },
        ]}
        dataSource={data}
      />
    </Layout>
  );
}

export default Page;
