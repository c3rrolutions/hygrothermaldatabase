import { Scalars } from "../../../__generated__/__types__";
import { usePhotovoltaicDataQuery } from "../../../queries/data.graphql";
import { Skeleton, Result, Descriptions, Typography } from "antd";
import { PageHeader } from "@ant-design/pro-layout";
import { useEffect } from "react";
import Link from "next/link";
import paths from "../../../paths";
import { messageApolloError } from "../../../lib/apollo";

export type PhotovoltaicDataProps = {
  photovoltaicDataId: Scalars["Uuid"];
};

export default function PhotovoltaicData({ photovoltaicDataId }: PhotovoltaicDataProps) {
  const { loading, error, data } = usePhotovoltaicDataQuery({
    variables: {
      uuid: photovoltaicDataId,
    },
  });
  const photovoltaicData = data?.photovoltaicData;

  useEffect(() => {
    if (error) {
      messageApolloError(error);
    }
  }, [error]);

  if (loading) {
    return <Skeleton active avatar title />;
  }

  if (!photovoltaicData) {
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
      title={photovoltaicData.name}
      subTitle={photovoltaicData.description}
      // extra={[
      //   <UpdatePhotovoltaicData
      //     key="updatePhotovoltaicData"
      //     photovoltaicDataId={photovoltaicData.uuid}
      //   />,
      // ]}
      backIcon={false}
    >
      <Descriptions size="small" column={1}>
        <Descriptions.Item label="UUID">{photovoltaicData.uuid}</Descriptions.Item>
        <Descriptions.Item label="Component">
          <Typography.Link
            href={paths.metabase.component(photovoltaicData.componentId)}
          >
            {photovoltaicData.componentId}
          </Typography.Link>
        </Descriptions.Item>
        <Descriptions.Item label="Creator">
          <Link
            href={paths.metabase.institution(photovoltaicData.creatorId)}
            legacyBehavior>
            {photovoltaicData.creatorId}
          </Link>
        </Descriptions.Item>
        <Descriptions.Item label="Description">{photovoltaicData.description}</Descriptions.Item>
        <Descriptions.Item label="Created At">{photovoltaicData.createdAt}</Descriptions.Item>
      </Descriptions>
    </PageHeader>
  );
}
