import type React from "react";
import "./Media.page.module.css";
import { useParams } from "react-router-dom";
import { Image, Container, AspectRatio, Text, Modal } from "@mantine/core";
import { useViewportSize } from "@mantine/hooks";
import { useNavigate } from "react-router-dom";

const MediaDetailsPage: React.FC = () => {
	const params = useParams();
	const { height, width } = useViewportSize();
	const navigate = useNavigate();
	return (
		<Modal
			opened={true}
			withCloseButton={false}
			onClose={() => {
				navigate("/media");
			}}
			fullScreen
		>
			<div
				style={{
					margin: 0,
					padding: 0,
					overflow: "hidden",
					height: "90vh",
					width: "98vw",
					display: "flex",
					justifyContent: "center",
					alignItems: "center",
				}}
			>
				<img
					style={{ maxHeight: "100%", maxWidth: "100%", objectFit: "contain" }}
					src={`/api/things/data/${params.id}/Preview_Original`}
				/>
			</div>
		</Modal>
	);
};

export default MediaDetailsPage;
