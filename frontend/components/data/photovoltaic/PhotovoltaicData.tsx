import { Scalars } from "../../../__generated__/graphql";
import { PhotovoltaicDataDocument } from "../../../queries/data.generated";
import { Skeleton, Result, App } from "antd";
import { useEffect } from "react";
import { stringifyApolloError } from "../../../lib/apollo";
import DataPageHeader from "../DataPageHeader";
import { useQuery } from "@apollo/client/react";

export type PhotovoltaicDataProps = {
  photovoltaicDataId: Scalars["Uuid"]["input"];
};

export default function PhotovoltaicData({
  photovoltaicDataId,
}: PhotovoltaicDataProps) {
  const { loading, error, data } = useQuery(PhotovoltaicDataDocument, {
    variables: {
      uuid: photovoltaicDataId,
    },
  });
  const photovoltaicData = data?.photovoltaicData;
  const { message } = App.useApp();

  useEffect(() => {
    if (error) {
      message.error(stringifyApolloError(error));
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
    <>
      <DataPageHeader
        data={photovoltaicData}
        // extra={[
        //   <UpdatePhotovoltaicData
        //     key="updatePhotovoltaicData"
        //     photovoltaicDataId={photovoltaicData.uuid}
        //   />,
        // ]}
      ></DataPageHeader>
    </>
  );
}
