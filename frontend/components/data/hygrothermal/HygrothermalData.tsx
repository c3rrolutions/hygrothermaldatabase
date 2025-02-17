import { Scalars } from "../../../__generated__/__types__";
import { useHygrothermalDataQuery } from "../../../queries/data.graphql";
import { Skeleton, Result, Descriptions, Typography } from "antd";
import { PageHeader } from "@ant-design/pro-layout";
import { useEffect } from "react";
import Link from "next/link";
import paths from "../../../paths";
import { messageApolloError } from "../../../lib/apollo";

export type HygrothermalDataProps = {
  hygrothermalDataId: Scalars["Uuid"];
};

export default function HygrothermalData({ hygrothermalDataId }: HygrothermalDataProps) {
  const { loading, error, data } = useHygrothermalDataQuery({
    variables: {
      uuid: hygrothermalDataId,
    },
  });
  const hygrothermalData = data?.hygrothermalData;

  useEffect(() => {
    if (error) {
      messageApolloError(error);
    }
  }, [error]);

  if (loading) {
    return <Skeleton active avatar title />;
  }

  if (!hygrothermalData) {
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
      title={hygrothermalData.name}
      subTitle={hygrothermalData.description}
      // extra={[
      //   <UpdateHygrothermalData
      //     key="updateHygrothermalData"
      //     hygrothermalDataId={hygrothermalData.uuid}
      //   />,
      // ]}
      backIcon={false}
    >
      <Descriptions size="small" column={1}>
        <Descriptions.Item label="UUID">{hygrothermalData.uuid}</Descriptions.Item>
        <Descriptions.Item label="Component">
          <Typography.Link
            href={paths.metabase.component(hygrothermalData.componentId)}
          >
            {hygrothermalData.componentId}
          </Typography.Link>
        </Descriptions.Item>
        <Descriptions.Item label="Creator">
          <Link
            href={paths.metabase.institution(hygrothermalData.creatorId)}
            legacyBehavior>
            {hygrothermalData.creatorId}
          </Link>
        </Descriptions.Item>
        <Descriptions.Item label="Description">{hygrothermalData.description}</Descriptions.Item>
        <Descriptions.Item label="Created At">{hygrothermalData.createdAt}</Descriptions.Item>
      </Descriptions>
    </PageHeader>
  );
}
