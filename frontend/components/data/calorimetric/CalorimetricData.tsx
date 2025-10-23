import { Scalars } from "../../../__generated__/__types__";
import { useCalorimetricDataQuery } from "../../../queries/data.graphql";
import { Skeleton, Result, Descriptions } from "antd";
import { useEffect } from "react";
import { messageApolloError } from "../../../lib/apollo";
import DataPageHeader from "../DataPageHeader";

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
    <DataPageHeader
      data={calorimetricData}
    // extra={[
    //   <UpdateCalorimetricData
    //     key="updateCalorimetricData"
    //     calorimetricDataId={calorimetricData.uuid}
    //   />,
    // ]}
    >
      <Descriptions.Item key="gValues" label="g Values">
        {calorimetricData.gValues.map((x) => x.toLocaleString("en")).join(", ")}
      </Descriptions.Item>
      <Descriptions.Item key="uValues" label="u Values">
        {calorimetricData.uValues.map((x) => x.toLocaleString("en")).join(", ")}
      </Descriptions.Item>
    </DataPageHeader>
  );
}
