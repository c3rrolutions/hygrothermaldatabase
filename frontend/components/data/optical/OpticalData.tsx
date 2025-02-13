import { Scalars } from "../../../__generated__/__types__";
import { useOpticalDataQuery } from "../../../queries/data.graphql";
import { Skeleton, Result, Descriptions, Typography } from "antd";
import { PageHeader } from "@ant-design/pro-layout";
import { useEffect } from "react";
import Link from "next/link";
import paths from "../../../paths";
import { messageApolloError } from "../../../lib/apollo";

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
    <PageHeader
      title={opticalData.name}
      subTitle={opticalData.description}
      // extra={[
      //   <UpdateOpticalData
      //     key="updateOpticalData"
      //     opticalDataId={opticalData.uuid}
      //   />,
      // ]}
      backIcon={false}
    >
      <Descriptions size="small" column={1}>
        <Descriptions.Item label="UUID">{opticalData.uuid}</Descriptions.Item>
        <Descriptions.Item label="Component">
          <Typography.Link
            href={paths.metabase.component(opticalData.componentId)}
          >
            {opticalData.componentId}
          </Typography.Link>
        </Descriptions.Item>
        <Descriptions.Item label="Creator">
          <Link href={paths.metabase.institution(opticalData.creatorId)} legacyBehavior>
            {opticalData.creatorId}
          </Link>
        </Descriptions.Item>
        <Descriptions.Item label="Description">{opticalData.description}</Descriptions.Item>
        <Descriptions.Item label="Created At">{opticalData.createdAt}</Descriptions.Item>
        <Descriptions.Item label="Applied Method">
          <Typography.Link
            href={paths.metabase.method(opticalData.appliedMethod.methodId)}
          >
            {opticalData.appliedMethod.methodId}
          </Typography.Link>
        </Descriptions.Item>
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
      </Descriptions>
    </PageHeader>
  );
}
