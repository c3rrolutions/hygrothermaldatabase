import { Scalars } from "../../../__generated__/__types__";
import { useCalorimetricDataQuery } from "../../../queries/data.graphql";
import { Skeleton, Result, Descriptions, Typography } from "antd";
import { PageHeader } from "@ant-design/pro-layout";
import { useEffect } from "react";
import Link from "next/link";
import paths from "../../../paths";
import { messageApolloError } from "../../../lib/apollo";

export type CalorimetricDataProps = {
  calorimetricDataId: Scalars["Uuid"];
};

export default function CalorimetricData({ calorimetricDataId }: CalorimetricDataProps) {
  const { loading, error, data } = useCalorimetricDataQuery({
    variables: {
      uuid: calorimetricDataId,
    },
  });
  const calorimetricData = data?.calorimetricData;

  useEffect(() => {
    if (error) {
      messageApolloError(error);
    }
  }, [error]);

  if (loading) {
    return <Skeleton active avatar title />;
  }

  if (!calorimetricData) {
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
      title={calorimetricData.name}
      subTitle={calorimetricData.description}
      // extra={[
      //   <UpdateCalorimetricData
      //     key="updateCalorimetricData"
      //     calorimetricDataId={calorimetricData.uuid}
      //   />,
      // ]}
      backIcon={false}
    >
      <Descriptions size="small" column={1}>
        <Descriptions.Item label="UUID">{calorimetricData.uuid}</Descriptions.Item>
        <Descriptions.Item label="Component">
          <Typography.Link
            href={paths.metabase.component(calorimetricData.componentId)}
          >
            {calorimetricData.componentId}
          </Typography.Link>
        </Descriptions.Item>
        <Descriptions.Item label="Creator">
          <Link
            href={paths.metabase.institution(calorimetricData.creatorId)}
            legacyBehavior>
            {calorimetricData.creatorId}
          </Link>
        </Descriptions.Item>
        <Descriptions.Item label="Description">{calorimetricData.description}</Descriptions.Item>
        <Descriptions.Item label="Created At">{calorimetricData.createdAt}</Descriptions.Item>
        <Descriptions.Item label="Applied Method">
          <Typography.Link
            href={paths.metabase.method(calorimetricData.appliedMethod.methodId)}
          >
            {calorimetricData.appliedMethod.methodId}
          </Typography.Link>
        </Descriptions.Item>
        <Descriptions.Item key="gValues" label="g Values">
          {calorimetricData.gValues.map((x) => x.toLocaleString("en")).join(", ")}
        </Descriptions.Item>
        <Descriptions.Item key="uValues" label="u Values">
          {calorimetricData.uValues.map((x) => x.toLocaleString("en")).join(", ")}
        </Descriptions.Item>
      </Descriptions>
    </PageHeader>
  );
}
