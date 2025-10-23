import { Scalars } from "../../../__generated__/__types__";
import { useOpticalDataQuery } from "../../../queries/data.graphql";
import { Skeleton, Result, Descriptions } from "antd";
import { useEffect } from "react";
import { messageApolloError } from "../../../lib/apollo";
import DataPageHeader from "../DataPageHeader";

export type OpticalDataProps = {
  opticalDataId: Scalars["Uuid"];
};

export default function OpticalData({ opticalDataId }: OpticalDataProps) {
  const { loading, error, data } = useOpticalDataQuery({
    variables: {
      uuid: opticalDataId,
    },
  });
  const opticalData = data?.opticalData;

  useEffect(() => {
    if (error) {
      messageApolloError(error);
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
    <DataPageHeader
      data={opticalData}
    // extra={[
    //   <UpdateOpticalData
    //     key="updateOpticalData"
    //     opticalDataId={opticalData.uuid}
    //   />,
    // ]}
    >
      <Descriptions.Item key="nearnormalHemisphericalVisibleTransmittances" label="Near Normal Hemispherical Visible Transmittances">
        {opticalData.nearnormalHemisphericalVisibleTransmittances.map((x) => x.toLocaleString("en")).join(", ")}
      </Descriptions.Item>
      <Descriptions.Item key="nearnormalHemisphericalVisibleReflectances" label="Near Normal Hemispherical Visible Reflectances">
        {opticalData.nearnormalHemisphericalVisibleReflectances.map((x) => x.toLocaleString("en")).join(", ")}
      </Descriptions.Item>
      <Descriptions.Item key="nearnormalHemisphericalSolarTransmittances" label="Near Normal Hemispherical Solar Transmittances">
        {opticalData.nearnormalHemisphericalSolarTransmittances.map((x) => x.toLocaleString("en")).join(", ")}
      </Descriptions.Item>
      <Descriptions.Item key="nearnormalHemisphericalSolarReflectances" label="Near Normal Hemispherical Solar Reflectances">
        {opticalData.nearnormalHemisphericalSolarReflectances.map((x) => x.toLocaleString("en")).join(", ")}
      </Descriptions.Item>
      <Descriptions.Item key="infraredEmittances" label="Infrared Emittances">
        {opticalData.infraredEmittances.map((x) => x.toLocaleString("en")).join(", ")}
      </Descriptions.Item>
      <Descriptions.Item key="colorRenderingIndices" label="Color Rendering Indices">
        {opticalData.colorRenderingIndices.map((x) => x.toLocaleString("en")).join(", ")}
      </Descriptions.Item>
    </DataPageHeader>
  );
}
