import { Scalars } from "../../../__generated__/graphql";
import { OpticalDataDocument } from "../../../queries/data.generated";
import { Skeleton, Result, Descriptions, message } from "antd";
import { useEffect } from "react";
import { stringifyApolloError } from "../../../lib/apollo";
import DataPageHeader from "../DataPageHeader";
import { useQuery } from "@apollo/client/react";

export type OpticalDataProps = {
  opticalDataId: Scalars["Uuid"]["input"];
};

export default function OpticalData({ opticalDataId }: OpticalDataProps) {
  const { loading, error, data } = useQuery(OpticalDataDocument, {
    variables: {
      uuid: opticalDataId,
    },
  });
  const opticalData = data?.opticalData;
  const [messageApi, contextHolder] = message.useMessage();

  useEffect(() => {
    if (error) {
      messageApi.error(stringifyApolloError(error));
    }
  }, [error]);

  if (loading) {
    return <Skeleton active avatar title />;
  }

  if (!opticalData) {
    return (
      <Result
        status="500"
        title="500"
        subTitle="Sorry, something went wrong."
      />
    );
  }

  return (
    <>
      {contextHolder}
      <DataPageHeader
        data={opticalData}
        // extra={[
        //   <UpdateOpticalData
        //     key="updateOpticalData"
        //     opticalDataId={opticalData.uuid}
        //   />,
        // ]}
      >
        <Descriptions.Item
          key="nearnormalHemisphericalVisibleTransmittances"
          label="Near Normal Hemispherical Visible Transmittances"
        >
          {opticalData.nearnormalHemisphericalVisibleTransmittances
            .map((x) => x.toLocaleString("en"))
            .join(", ")}
        </Descriptions.Item>
        <Descriptions.Item
          key="nearnormalHemisphericalVisibleReflectances"
          label="Near Normal Hemispherical Visible Reflectances"
        >
          {opticalData.nearnormalHemisphericalVisibleReflectances
            .map((x) => x.toLocaleString("en"))
            .join(", ")}
        </Descriptions.Item>
        <Descriptions.Item
          key="nearnormalHemisphericalSolarTransmittances"
          label="Near Normal Hemispherical Solar Transmittances"
        >
          {opticalData.nearnormalHemisphericalSolarTransmittances
            .map((x) => x.toLocaleString("en"))
            .join(", ")}
        </Descriptions.Item>
        <Descriptions.Item
          key="nearnormalHemisphericalSolarReflectances"
          label="Near Normal Hemispherical Solar Reflectances"
        >
          {opticalData.nearnormalHemisphericalSolarReflectances
            .map((x) => x.toLocaleString("en"))
            .join(", ")}
        </Descriptions.Item>
        <Descriptions.Item key="infraredEmittances" label="Infrared Emittances">
          {opticalData.infraredEmittances
            .map((x) => x.toLocaleString("en"))
            .join(", ")}
        </Descriptions.Item>
        <Descriptions.Item
          key="colorRenderingIndices"
          label="Color Rendering Indices"
        >
          {opticalData.colorRenderingIndices
            .map((x) => x.toLocaleString("en"))
            .join(", ")}
        </Descriptions.Item>
      </DataPageHeader>
    </>
  );
}
